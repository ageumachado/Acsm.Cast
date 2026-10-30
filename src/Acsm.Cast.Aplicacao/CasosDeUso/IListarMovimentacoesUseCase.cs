using Acsm.Cast.Aplicacao.DTOs;

namespace Acsm.Cast.Aplicacao.CasosDeUso;

public interface IListarMovimentacoesUseCase
{
    Task<IReadOnlyList<MovimentacaoResponse>> ExecutarAsync(string idConta, CancellationToken cancellationToken = default);
}
