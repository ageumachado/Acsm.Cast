namespace Acsm.Cast.Dominio.Excecoes;

public class ContaNaoEncontradaException : DominioException
{
    public ContaNaoEncontradaException(string idConta)
        : base($"Conta '{idConta}' não foi encontrada.") { }
}
