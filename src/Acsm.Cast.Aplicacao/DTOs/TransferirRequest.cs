namespace Acsm.Cast.Aplicacao.DTOs;

public record TransferirRequest(
    string? Origem,
    string? Destino,
    long ValorCentavos
);
