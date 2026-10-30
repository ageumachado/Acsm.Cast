using Acsm.Cast.Aplicacao.DTOs;

namespace Acsm.Cast.Aplicacao.CasosDeUso;

public interface IObterContaUseCase
{
    Task<ContaResponse> ExecutarAsync(string id, CancellationToken cancellationToken = default);
}
