using Acsm.Cast.Aplicacao.DTOs;
using Acsm.Cast.Dominio.Contratos;
using Acsm.Cast.Dominio.Excecoes;

namespace Acsm.Cast.Aplicacao.CasosDeUso;

public class ListarMovimentacoesUseCase : IListarMovimentacoesUseCase
{
    private readonly IContaRepository _contaRepository;
    private readonly ITransferenciaRepository _transferenciaRepository;

    public ListarMovimentacoesUseCase(
        IContaRepository contaRepository,
        ITransferenciaRepository transferenciaRepository)
    {
        _contaRepository = contaRepository;
        _transferenciaRepository = transferenciaRepository;
    }

    public async Task<IReadOnlyList<MovimentacaoResponse>> ExecutarAsync(string idConta, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(idConta))
            throw new RegraNegocioException("O identificador da conta deve ser informado.");

        var contaId = idConta.Trim().ToUpperInvariant();

        // Valida se a conta existe antes de buscar movimentações (HTTP 404 para inexistente)
        var conta = await _contaRepository.ObterPorIdAsync(contaId, cancellationToken)
            ?? throw new ContaNaoEncontradaException(contaId);

        var movimentacoes = await _transferenciaRepository.ListarPorContaAsync(conta.Id, cancellationToken);

        return [.. movimentacoes
            .Select(m => new MovimentacaoResponse(
                m.Id,
                m.OrigemId,
                m.DestinoId,
                m.ValorCentavos,
                m.CriadoEmUtc))];
    }
}
