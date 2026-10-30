using Acsm.Cast.Dominio.Entidades;

namespace Acsm.Cast.Dominio.Contratos;

public interface IContaRepository
{
    Task<Conta?> ObterPorIdAsync(string id, CancellationToken cancellationToken = default);
    Task AtualizarAsync(Conta conta, CancellationToken cancellationToken = default);
}
