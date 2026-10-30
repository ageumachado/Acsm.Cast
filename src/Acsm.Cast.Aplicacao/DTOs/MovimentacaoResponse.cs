namespace Acsm.Cast.Aplicacao.DTOs;

public record MovimentacaoResponse(
    Guid Id,
    string Origem,
    string Destino,
    long ValorCentavos,
    DateTime CriadoEmUtc
);
