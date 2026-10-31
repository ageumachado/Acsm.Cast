using Acsm.Cast.Dominio.Contratos;
using Acsm.Cast.Dominio.Entidades;
using Acsm.Cast.Infra.Contexto;
using Microsoft.EntityFrameworkCore;

namespace Acsm.Cast.Infra.Repositorios;

public class TransferenciaRepository(CastDbContext context) : ITransferenciaRepository
{
    public async Task AdicionarAsync(Transferencia transferencia, CancellationToken cancellationToken = default)
    {
        await context.Transferencias.AddAsync(transferencia, cancellationToken);
    }

    public async Task<IReadOnlyList<Transferencia>> ListarPorContaAsync(string contaId, CancellationToken cancellationToken = default)
    {
        return await context.Transferencias
            .AsNoTracking()
            .Where(t => t.OrigemId == contaId || t.DestinoId == contaId)
            .OrderBy(t => t.Id)
            .ToListAsync(cancellationToken);
    }
}
