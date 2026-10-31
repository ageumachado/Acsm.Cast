using Acsm.Cast.Questao2.Tests.Modelos;

namespace Acsm.Cast.Questao2.Tests;

public class SimuladorOriginal
{
    public List<RegistroMovimentacao> Movimentacoes { get; } = [];

    public void Transferir(ContaMemoria origem, ContaMemoria destino, long valor)
    {
        if (valor <= 0)
            throw new ArgumentException("valor inválido");

        if (origem.Saldo < valor)
            throw new InvalidOperationException("saldo insuficiente");

        // FALHA IDENTIFICADA: débito efetuado antes de validar se o crédito pode ocorrer
        origem.Saldo -= valor;

        Creditar(destino, valor);

        RegistrarMovimentacao(origem.Id, destino.Id, valor);
    }

    private static void Creditar(ContaMemoria destino, long valor)
    {
        if (destino.Bloqueada)
            throw new InvalidOperationException("destino bloqueado");

        destino.Saldo += valor;
    }

    private void RegistrarMovimentacao(string origem, string destino, long valor)
    {
        Movimentacoes.Add(new RegistroMovimentacao(origem, destino, valor));
    }
}
