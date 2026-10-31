using Acsm.Cast.Aplicacao.CasosDeUso;
using Acsm.Cast.Aplicacao.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Acsm.Cast.Api.Endpoints;

public static class TransferenciaEndpoints
{
    extension(IEndpointRouteBuilder app)
    {
        // -----------------------------------------------------------------------------
        // Endpoints (Minimal APIs)
        // -----------------------------------------------------------------------------
        public void MapearEndpoints()
        {
            // 1. POST /transferencias
            // Recebe origem, destino e valorCentavos; retorna 201 com o ID criado.
            app.MapPost("/transferencias", async (
                [FromBody] TransferirRequest request,
                [FromServices] IRealizarTransferenciaUseCase useCase,
                CancellationToken ct) =>
            {
                var resultado = await useCase.ExecutarAsync(request, ct);
                return Results.Created($"/transferencias/{resultado.Id}", resultado);
            })
            .WithName("RealizarTransferencia")
            .Produces<TransferenciaResponse>(StatusCodes.Status201Created)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
            .Produces<ProblemDetails>(StatusCodes.Status409Conflict);

            // 2. GET /contas/{id}
            // Retorna ID e saldoCentavos atual; 404 para conta inexistente.
            app.MapGet("/contas/{id}", async (
                string id,
                [FromServices] IObterContaUseCase useCase,
                CancellationToken ct) =>
            {
                var conta = await useCase.ExecutarAsync(id, ct);
                return Results.Ok(conta);
            })
            .WithName("ObterContaPorId")
            .Produces<ContaResponse>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound);

            // 3. GET /contas/{id}/movimentacoes
            // Lista as transferências dessa conta ordenadas por ID; 404 para conta inexistente.
            app.MapGet("/contas/{id}/movimentacoes", async (
                string id,
                [FromServices] IListarMovimentacoesUseCase useCase,
                CancellationToken ct) =>
            {
                var movimentacoes = await useCase.ExecutarAsync(id, ct);
                return Results.Ok(movimentacoes);
            })
            .WithName("ListarMovimentacoesDaConta")
            .Produces<IReadOnlyList<MovimentacaoResponse>>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound);
        }
    }
}
