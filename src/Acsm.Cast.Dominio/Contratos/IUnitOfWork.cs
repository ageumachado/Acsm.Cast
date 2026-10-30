namespace Acsm.Cast.Dominio.Contratos;

public interface IUnitOfWork
{
    Task CommitAsync(CancellationToken cancellationToken = default);
}
