using Acsm.Cast.Dominio.Contratos;
using Acsm.Cast.Dominio.Entidades;
using Acsm.Cast.Infra.Contexto;
using Microsoft.EntityFrameworkCore;

namespace Acsm.Cast.Infra.Repositorios;

public class TransferenciaRepository : ITransferenciaRepository
{
    private readonly CastDbContext _context;

    public TransferenciaRepository(CastDbContext context)
    {
        _context = context;
    }

    public async Task AdicionarAsync(Transferencia transferencia, CancellationToken cancellationToken = default)
    {
        await _context.Transferencias.AddAsync(transferencia, cancellationToken);
    }

    public async Task<IReadOnlyList<Transferencia>> ListarPorContaAsync(string contaId, CancellationToken cancellationToken = default)
    {
        return await _context.Transferencias
            .AsNoTracking()
            .Where(t => t.OrigemId == contaId || t.DestinoId == contaId)
            .OrderBy(t => t.Id)
            .ToListAsync(cancellationToken);
    }
}
