using Acsm.Cast.Dominio.Contratos;
using Acsm.Cast.Dominio.Entidades;
using Acsm.Cast.Infra.Contexto;
using Microsoft.EntityFrameworkCore;

namespace Acsm.Cast.Infra.Repositorios;

public class ContaRepository(CastDbContext context) : IContaRepository
{
    public async Task<Conta?> ObterPorIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return await context.Contas
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public Task AtualizarAsync(Conta conta, CancellationToken cancellationToken = default)
    {
        context.Contas.Update(conta);
        return Task.CompletedTask;
    }
}
