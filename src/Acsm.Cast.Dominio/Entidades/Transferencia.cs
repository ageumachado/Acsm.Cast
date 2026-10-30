using Acsm.Cast.Dominio.Excecoes;

namespace Acsm.Cast.Dominio.Entidades;

public class Transferencia
{
    public Guid Id { get; private set; }
    public string OrigemId { get; private set; } = string.Empty;
    public string DestinoId { get; private set; } = string.Empty;
    public long ValorCentavos { get; private set; }
    public DateTime CriadoEmUtc { get; private set; }

    protected Transferencia() { }

    public Transferencia(string origemId, string destinoId, long valorCentavos)
    {
        if (string.IsNullOrWhiteSpace(origemId))
            throw new RegraNegocioException("A conta de origem é obrigatória.");

        if (string.IsNullOrWhiteSpace(destinoId))
            throw new RegraNegocioException("A conta de destino é obrigatória.");

        var origemNormalizada = origemId.Trim().ToUpperInvariant();
        var destinoNormalizado = destinoId.Trim().ToUpperInvariant();

        if (origemNormalizada.Equals(destinoNormalizado, StringComparison.Ordinal))
            throw new RegraNegocioException("A conta de origem não pode ser igual à conta de destino.");

        if (valorCentavos <= 0)
            throw new RegraNegocioException("O valor da transferência deve ser maior que zero.");

        Id = Guid.NewGuid();
        OrigemId = origemNormalizada;
        DestinoId = destinoNormalizado;
        ValorCentavos = valorCentavos;
        CriadoEmUtc = DateTime.UtcNow;
    }
}
