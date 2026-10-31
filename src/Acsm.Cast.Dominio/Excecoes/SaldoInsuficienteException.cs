namespace Acsm.Cast.Dominio.Excecoes;

public class SaldoInsuficienteException : DominioException
{
    public SaldoInsuficienteException(string idConta, long saldoAtual, long valorTentado)
        : base($"Conta '{idConta}' possui saldo insuficiente ({saldoAtual} centavos) para o débito de {valorTentado} centavos.") { }
}
