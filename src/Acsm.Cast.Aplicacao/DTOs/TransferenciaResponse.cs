namespace Acsm.Cast.Aplicacao.DTOs;

public record TransferenciaResponse(
    Guid Id,
    string Origem,
    string Destino,
    long ValorCentavos,
    DateTime CriadoEmUtc
);
