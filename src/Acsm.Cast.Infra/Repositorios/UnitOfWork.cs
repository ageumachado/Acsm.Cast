using Acsm.Cast.Dominio.Contratos;
using Acsm.Cast.Infra.Contexto;

namespace Acsm.Cast.Infra.Repositorios;

public class UnitOfWork(CastDbContext context) : IUnitOfWork
{
    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        await context.SaveChangesAsync(cancellationToken);
    }
}
