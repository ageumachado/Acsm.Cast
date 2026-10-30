using Acsm.Cast.Dominio.Contratos;
using Acsm.Cast.Infra.Contexto;

namespace Acsm.Cast.Infra.Repositorios;

public class UnitOfWork : IUnitOfWork
{
    private readonly CastDbContext _context;

    public UnitOfWork(CastDbContext context)
    {
        _context = context;
    }

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
