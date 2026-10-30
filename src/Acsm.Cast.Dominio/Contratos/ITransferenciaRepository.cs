using Acsm.Cast.Dominio.Entidades;

namespace Acsm.Cast.Dominio.Contratos;

public interface ITransferenciaRepository
{
    Task AdicionarAsync(Transferencia transferencia, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Transferencia>> ListarPorContaAsync(string contaId, CancellationToken cancellationToken = default);
}
