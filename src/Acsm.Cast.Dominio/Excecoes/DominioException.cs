namespace Acsm.Cast.Dominio.Excecoes;

public abstract class DominioException : Exception
{
    protected DominioException(string mensagem) : base(mensagem) { }
}

public class RegraNegocioException : DominioException
{
    public RegraNegocioException(string mensagem) : base(mensagem) { }
}

public class ContaNaoEncontradaException : DominioException
{
    public ContaNaoEncontradaException(string idConta)
        : base($"Conta '{idConta}' não foi encontrada.") { }
}

public class SaldoInsuficienteException : DominioException
{
    public SaldoInsuficienteException(string idConta, long saldoAtual, long valorTentado)
        : base($"Conta '{idConta}' possui saldo insuficiente ({saldoAtual} centavos) para o débito de {valorTentado} centavos.") { }
}
