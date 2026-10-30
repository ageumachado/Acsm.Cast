namespace Acsm.Cast.Questao2.Tests;

public class ContaMemoria
{
    public string Id { get; init; } = string.Empty;
    public long Saldo { get; set; }
    public bool Bloqueada { get; set; }
}

public record RegistroMovimentacao(
    string Origem,
    string Destino,
    long Valor
);

public class SimuladorOriginal
{
    public List<RegistroMovimentacao> Movimentacoes { get; } = new();

    public void Transferir(ContaMemoria origem, ContaMemoria destino, long valor)
    {
        if (valor <= 0)
            throw new ArgumentException("valor inválido");

        if (origem.Saldo < valor)
            throw new InvalidOperationException("saldo insuficiente");

        // FALHA IDENTIFICADA: débito efetuado antes de validar se o crédito pode ocorrer
        origem.Saldo = origem.Saldo - valor;

        Creditar(destino, valor);

        RegistrarMovimentacao(origem.Id, destino.Id, valor);
    }

    private void Creditar(ContaMemoria destino, long valor)
    {
        if (destino.Bloqueada)
            throw new InvalidOperationException("destino bloqueado");

        destino.Saldo = destino.Saldo + valor;
    }

    private void RegistrarMovimentacao(string origem, string destino, long valor)
    {
        Movimentacoes.Add(new RegistroMovimentacao(origem, destino, valor));
    }
}

public class ServicoCorrigido
{
    public List<RegistroMovimentacao> Movimentacoes { get; } = new();

    public void Transferir(ContaMemoria origem, ContaMemoria destino, long valor)
    {
        // 1. Fase de Validações Prévias (Fail-Fast antes de qualquer efeito colateral)
        if (valor <= 0)
            throw new ArgumentException("valor inválido");

        if (origem.Saldo < valor)
            throw new InvalidOperationException("saldo insuficiente");

        if (destino.Bloqueada)
            throw new InvalidOperationException("destino bloqueado");

        // 2. Fase de Mutações (executada somente quando nenhuma pré-condição falhou)
        origem.Saldo -= valor;
        destino.Saldo += valor;

        RegistrarMovimentacao(origem.Id, destino.Id, valor);
    }

    private void RegistrarMovimentacao(string origem, string destino, long valor)
    {
        Movimentacoes.Add(new RegistroMovimentacao(origem, destino, valor));
    }
}

public class FalhaECorrecaoTests
{
    [Fact]
    public void SimuladorOriginal_QuandoDestinoBloqueado_DemonstraFalhaComPerdaDeSaldo()
    {
        // Arrange: Cenário exato do enunciado (A=10000, B=5000, B bloqueada, valor 3000)
        var servico = new SimuladorOriginal();
        var contaA = new ContaMemoria { Id = "A", Saldo = 10000, Bloqueada = false };
        var contaB = new ContaMemoria { Id = "B", Saldo = 5000, Bloqueada = true };

        // Act & Assert
        var ex = Assert.Throws<InvalidOperationException>(() => servico.Transferir(contaA, contaB, 3000));
        Assert.Equal("destino bloqueado", ex.Message);

        // EVIDÊNCIA DO BUG: A perdeu 3000 indevidamente e ficou com 7000, B continuou com 5000
        Assert.Equal(7000, contaA.Saldo);
        Assert.Equal(5000, contaB.Saldo);
        Assert.Empty(servico.Movimentacoes);
    }

    [Fact]
    public void ServicoCorrigido_QuandoDestinoBloqueado_MantemSaldosIntactosESemMovimentacao()
    {
        // Arrange: Mesmo cenário com o serviço corrigido
        var servico = new ServicoCorrigido();
        var contaA = new ContaMemoria { Id = "A", Saldo = 10000, Bloqueada = false };
        var contaB = new ContaMemoria { Id = "B", Saldo = 5000, Bloqueada = true };

        // Act & Assert
        var ex = Assert.Throws<InvalidOperationException>(() => servico.Transferir(contaA, contaB, 3000));
        Assert.Equal("destino bloqueado", ex.Message);

        // CORREÇÃO COMPROVADA: Nenhum saldo foi alterado (A=10000, B=5000)
        Assert.Equal(10000, contaA.Saldo);
        Assert.Equal(5000, contaB.Saldo);
        Assert.Empty(servico.Movimentacoes);
    }

    [Fact]
    public void ServicoCorrigido_QuandoDestinoDesbloqueado_RealizaTransferenciaComSucesso()
    {
        // Arrange
        var servico = new ServicoCorrigido();
        var contaA = new ContaMemoria { Id = "A", Saldo = 10000, Bloqueada = false };
        var contaB = new ContaMemoria { Id = "B", Saldo = 5000, Bloqueada = false };

        // Act
        servico.Transferir(contaA, contaB, 3000);

        // Assert
        Assert.Equal(7000, contaA.Saldo);
        Assert.Equal(8000, contaB.Saldo);
        Assert.Single(servico.Movimentacoes);

        var mov = servico.Movimentacoes[0];
        Assert.Equal("A", mov.Origem);
        Assert.Equal("B", mov.Destino);
        Assert.Equal(3000, mov.Valor);
    }

    [Fact]
    public void ServicoCorrigido_QuandoSaldoInsuficiente_LancaExcecaoEMantemSaldosIntactos()
    {
        // Arrange
        var servico = new ServicoCorrigido();
        var contaA = new ContaMemoria { Id = "A", Saldo = 2000, Bloqueada = false };
        var contaB = new ContaMemoria { Id = "B", Saldo = 5000, Bloqueada = false };

        // Act & Assert
        var ex = Assert.Throws<InvalidOperationException>(() => servico.Transferir(contaA, contaB, 3000));
        Assert.Equal("saldo insuficiente", ex.Message);

        Assert.Equal(2000, contaA.Saldo);
        Assert.Equal(5000, contaB.Saldo);
        Assert.Empty(servico.Movimentacoes);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-100)]
    public void ServicoCorrigido_QuandoValorInvalido_LancaArgumentException(long valorInvalido)
    {
        // Arrange
        var servico = new ServicoCorrigido();
        var contaA = new ContaMemoria { Id = "A", Saldo = 10000, Bloqueada = false };
        var contaB = new ContaMemoria { Id = "B", Saldo = 5000, Bloqueada = false };

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => servico.Transferir(contaA, contaB, valorInvalido));
        Assert.Equal("valor inválido", ex.Message);

        Assert.Equal(10000, contaA.Saldo);
        Assert.Equal(5000, contaB.Saldo);
    }
}