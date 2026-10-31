using Acsm.Cast.Aplicacao.DTOs;
using Acsm.Cast.Dominio.Contratos;
using Acsm.Cast.Dominio.Excecoes;

namespace Acsm.Cast.Aplicacao.CasosDeUso;

public class ObterContaUseCase : IObterContaUseCase
{
    private readonly IContaRepository _contaRepository;

    public ObterContaUseCase(IContaRepository contaRepository)
    {
        _contaRepository = contaRepository;
    }

    public async Task<ContaResponse> ExecutarAsync(string id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new RegraNegocioException("O identificador da conta deve ser informado.");

        var contaId = id.Trim().ToUpperInvariant();

        var conta = await _contaRepository.ObterPorIdAsync(contaId, cancellationToken)
            ?? throw new ContaNaoEncontradaException(contaId);

        return new ContaResponse(conta.Id, conta.SaldoCentavos);
    }
}
