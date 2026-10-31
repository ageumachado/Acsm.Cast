namespace Acsm.Cast.Dominio.Excecoes;

public abstract class DominioException : Exception
{
    protected DominioException(string mensagem) : base(mensagem) { }
}
