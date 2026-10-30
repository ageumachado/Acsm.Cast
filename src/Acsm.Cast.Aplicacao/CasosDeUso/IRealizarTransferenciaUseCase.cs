using Acsm.Cast.Aplicacao.DTOs;

namespace Acsm.Cast.Aplicacao.CasosDeUso;

public interface IRealizarTransferenciaUseCase
{
    Task<TransferenciaResponse> ExecutarAsync(TransferirRequest request, CancellationToken cancellationToken = default);
}
