using Acsm.Cast.Aplicacao.DTOs;
using Acsm.Cast.Dominio.Contratos;
using Acsm.Cast.Dominio.Entidades;
using Acsm.Cast.Dominio.Excecoes;

namespace Acsm.Cast.Aplicacao.CasosDeUso;

public class RealizarTransferenciaUseCase : IRealizarTransferenciaUseCase
{
    private readonly IContaRepository _contaRepository;
    private readonly ITransferenciaRepository _transferenciaRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RealizarTransferenciaUseCase(
        IContaRepository contaRepository,
        ITransferenciaRepository transferenciaRepository,
        IUnitOfWork unitOfWork)
    {
        _contaRepository = contaRepository;
        _transferenciaRepository = transferenciaRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<TransferenciaResponse> ExecutarAsync(TransferirRequest request, CancellationToken cancellationToken = default)
    {
        // Validações defensivas de entrada (mapeadas para HTTP 400)
        if (string.IsNullOrWhiteSpace(request.Origem))
            throw new RegraNegocioException("O campo 'origem' é obrigatório.");

        if (string.IsNullOrWhiteSpace(request.Destino))
            throw new RegraNegocioException("O campo 'destino' é obrigatório.");

        if (request.ValorCentavos <= 0)
            throw new RegraNegocioException("O campo 'valorCentavos' deve ser maior que zero.");

        var origemId = request.Origem.Trim().ToUpperInvariant();
        var destinoId = request.Destino.Trim().ToUpperInvariant();

        if (origemId.Equals(destinoId, StringComparison.Ordinal))
            throw new RegraNegocioException("A conta de origem não pode ser idêntica à conta de destino.");

        // Obtenção e verificação de existência das contas (mapeadas para HTTP 404)
        var contaOrigem = await _contaRepository.ObterPorIdAsync(origemId, cancellationToken)
            ?? throw new ContaNaoEncontradaException(origemId);

        var contaDestino = await _contaRepository.ObterPorIdAsync(destinoId, cancellationToken)
            ?? throw new ContaNaoEncontradaException(destinoId);

        // Operações de domínio (débito dispara SaldoInsuficienteException se saldo < valor)
        contaOrigem.Debitar(request.ValorCentavos);
        contaDestino.Creditar(request.ValorCentavos);

        var transferencia = new Transferencia(contaOrigem.Id, contaDestino.Id, request.ValorCentavos);

        // Persistência em lote
        await _contaRepository.AtualizarAsync(contaOrigem, cancellationToken);
        await _contaRepository.AtualizarAsync(contaDestino, cancellationToken);
        await _transferenciaRepository.AdicionarAsync(transferencia, cancellationToken);

        // Commit da transação via Unit of Work
        await _unitOfWork.CommitAsync(cancellationToken);

        return new TransferenciaResponse(
            transferencia.Id,
            transferencia.OrigemId,
            transferencia.DestinoId,
            transferencia.ValorCentavos,
            transferencia.CriadoEmUtc
        );
    }
}
