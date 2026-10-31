using Acsm.Cast.Questao2.Tests.Modelos;

namespace Acsm.Cast.Questao2.Tests;

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
