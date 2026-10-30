# Acsm.Cast — Avaliação de Desenvolvimento C#

Solução desenvolvida em **.NET 10 (C# 14)** para avaliação técnica. O projeto aborda a **Questão 1** (API de transferências bancárias com persistência local SQLite, EF Core 10, Clean Architecture e DDD) e a **Questão 2** (investigação, correção e análise de falha de atomicidade e concorrência em módulo de teste isolado).

---

## 1. Estrutura da Solução

```text
Acsm.Cast/
├── src/
│   ├── Acsm.Cast.Dominio/          # [Q1] Entidades ricas, Value Objects, Exceções, Contratos
│   ├── Acsm.Cast.Aplicacao/        # [Q1] Casos de Uso (Transferência, Consulta, Extrato), DTOs
│   ├── Acsm.Cast.Infra/            # [Q1] EF Core 10, SQLite, Mapeamentos Fluent API, Seed inicial
│   └── Acsm.Cast.Api/              # [Q1] Minimal APIs, Exception Handler (400, 404, 409), OpenAPI
└── tests/
    ├── Acsm.Cast.Dominio.Tests/    # [Q1] Testes unitários das regras de negócio financeiras
    └── Acsm.Cast.Questao2.Tests/   # [Q2] Módulo isolado: demonstração do bug, correção e testes
```

---

## 2. Questão 1 — Mini API de Transferências

### Instruções de Execução

#### Pré-requisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/)
- Visual Studio 2026 (ou VS Code / JetBrains Rider)

#### Comandos via CLI (`dotnet`)

1. **Restaurar e compilar a solução:**

   ```bash
   dotnet restore Acsm.Cast.sln
   dotnet build Acsm.Cast.sln -c Release
   ```

2. **Executar a suíte de testes unitários da Questão 1:**

   ```bash
   dotnet test tests/Acsm.Cast.Dominio.Tests/Acsm.Cast.Dominio.Tests.csproj --logger "console;verbosity=detailed"
   ```

3. **Executar a API localmente:**
   ```bash
   dotnet run --project src/Acsm.Cast.Api/Acsm.Cast.Api.csproj
   ```

A API inicializará em `http://localhost:5000` (ou na porta configurada pelo Kestrel). O banco de dados SQLite local (`cast_transferencias.db`) é criado e semeado automaticamente na inicialização com os dados de avaliação:

- Conta **A**: `100000` centavos (R$ 1.000,00)
- Conta **B**: `50000` centavos (R$ 500,00)

> **Dica Visual Studio 2026:** Utilize o arquivo `src/Acsm.Cast.Api/transferencias.http` para disparar os testes de requisição diretamente pelo editor.

### Endpoints e Exemplos de Requisições

- **`POST /transferencias`**:
  - **Payload:** `{"origem": "A", "destino": "B", "valorCentavos": 3000}`
  - **201 Created:** Transferência efetuada (retorna `id`, `origem`, `destino`, `valorCentavos`, `criadoEmUtc`).
  - **400 Bad Request:** Valor zero, negativo, decimal, contas iguais ou campos ausentes.
  - **404 Not Found:** Conta de origem ou destino inexistente.
  - **409 Conflict:** Saldo insuficiente (saldos permanecem inalterados).
- **`GET /contas/{id}`**:
  - **200 OK:** Retorna `id` e `saldoCentavos` atual.
  - **404 Not Found:** Conta inexistente.
- **`GET /contas/{id}/movimentacoes`**:
  - **200 OK:** Lista transferências da conta (origem ou destino), ordenadas crescentemente por ID.
  - **404 Not Found:** Conta inexistente.

### Decisões Técnicas da Questão 1

- **Clean Architecture & DDD:** O núcleo do domínio (`Acsm.Cast.Dominio`) não possui dependências de infraestrutura. As entidades encapsulam suas regras de validação (`Debitar`, `Creditar`), e as exceções de domínio são mapeadas centralizadamente para códigos RFC 7807 (`ProblemDetails`) na API.
- **Valores Inteiros de Centavos (`long`):** Evita imprecisão e arredondamentos inerentes a tipos de ponto flutuante binário (`float` ou `double`), garantindo precisão estrita em operações financeiras.
- **Persistência SQLite vs In-Memory:** Optou-se pelo SQLite local ao invés do provider `InMemory` do EF Core porque o SQLite simula com fidelidade o comportamento relacional, restrições de integridade e suporta transações reais ACID via `UnitOfWork` (`SaveChangesAsync`), assegurando que débito, crédito e registro de movimentação persistam juntos ou sejam revertidos por completo.

---

## 3. Questão 2 — Investigação e Correção de Falha em Memória

### Execução dos Testes da Questão 2

Para executar os testes que comprovam a falha original e a correção implementada:

```bash
dotnet test tests/Acsm.Cast.Questao2.Tests/Acsm.Cast.Questao2.Tests.csproj --logger "console;verbosity=detailed"
```

### Diagnóstico do Problema Original

O pseudocódigo original apresentava uma falha clássica de **quebra de atomicidade** por mutação prematura de estado:

1. O método debitava o saldo da conta de origem (`origem.saldo = origem.saldo - valor`).
2. Em seguida, invocava `creditar(destino, valor)`.
3. Se a conta de destino estivesse bloqueada (`destino.bloqueada == true`), uma exceção era lançada interrompendo o fluxo.
4. Como não havia transação em memória, o débito na origem permaneceu consolidado, deixando o sistema em estado inconsistente ($A=7000$ e $B=5000$).

### Correção Mínima Implementada

Ajustou-se a ordem de execução aplicando o princípio de **validação prévia completa (Fail-Fast / Check-Then-Act defensivo)**: nenhuma mutação de estado ocorre antes que todas as condições de guarda (saldo suficiente, conta de destino desbloqueada e valor válido) tenham sido verificadas.

---

## 4. Análise Crítica: Limitações da Correção Diante de Banco de Dados e Concorrência

Conforme solicitado no enunciado da Questão 2, a correção mínima em memória apresentada **não garante segurança nem integridade em um cenário produtivo real**. Os motivos técnicos são:

### 1. Vulnerabilidade a Falhas de Banco de Dados (Ausência de Atomicidade ACID Real)

- **Ponto de Falha de IO / Infraestrutura:** Em um banco de dados real, persistir a alteração de duas contas e registrar uma movimentação envolve chamadas de rede e gravação em disco. Mesmo que o código em C# faça todas as validações prévias na memória, a persistência envolve múltiplos comandos (`UPDATE conta_origem`, `UPDATE conta_destino`, `INSERT movimentacao`).
- **Inconsistência por Pane Parcial:** Se houver queda de energia, timeout de rede, estouro de disco ou reinicialização do banco de dados exatamente após o primeiro `UPDATE`, os saldos ficarão corrompidos.
- **Por que a validação do código não resolve:** Somente o suporte a **Transações ACID do banco de dados** (`BEGIN TRANSACTION ... COMMIT` com rollback automático via WAL/Journal) ou transações distribuídas (Saga / Two-Phase Commit) garantem que todas as operações persistam ou nenhuma persista diante de interrupções de infraestrutura.

### 2. Vulnerabilidade a Requisições Simultâneas (Condição de Corrida / Race Condition)

- **Problema TOCTOU (Time-of-Check to Time-of-Use):** A validação `if (origem.Saldo < valor)` verifica o saldo em um instante $T_0$, mas a dedução só ocorre em $T_1$.
- **Cenário de _Double Spending_:** Se uma conta com saldo de 10.000 centavos receber duas requisições simultâneas de transferência de 8.000 centavos em threads diferentes:
  1. Thread 1 lê saldo = 10.000 (Passa na validação).
  2. Thread 2 lê saldo = 10.000 (Passa na validação ao mesmo tempo).
  3. Thread 1 debita 8.000 $\rightarrow$ Saldo = 2.000.
  4. Thread 2 debita 8.000 $\rightarrow$ Saldo = -6.000.
- **Por que a validação do código não resolve:** Variáveis e propriedades em memória não são sincronizadas entre threads ou entre múltiplas instâncias de uma aplicação (_cluster/containers_).

### Soluções Necessárias em Ambiente Real

1. **Controle de Concorrência Otimista:** Uso de tokens de versão (`RowVersion` / `concurrency token`) que abortam a transação caso o registro tenha sido alterado entre a leitura e a escrita.
2. **Controle de Concorrência Pessimista:** Bloqueio de linha no banco de dados (`SELECT ... FOR UPDATE`) garantindo execução estritamente sequencial sobre a conta de origem.
3. **Idempotência com Chaves Únicas:** Utilização de `Idempotency-Key` em cache distribuído (ex.: Redis) para prevenir execuções duplicadas originadas por retentativas de rede.
4. **Outbox Pattern:** Envio confiável de notificações e histórico de eventos para corretores de mensageria (RabbitMQ/Kafka) para conciliação bancária desacoplada.
