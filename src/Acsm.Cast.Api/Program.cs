using Acsm.Cast.Api.Middlewares;
using Acsm.Cast.Aplicacao.CasosDeUso;
using Acsm.Cast.Aplicacao.DTOs;
using Acsm.Cast.Infra;
using Microsoft.AspNetCore.Mvc;
using Scalar.AspNetCore;
using Acsm.Cast.Aplicacao;

var builder = WebApplication.CreateBuilder(args);

// Configuração do SQLite a partir da ConnectionString ou fallback local
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                       ?? "Data Source=cast_transferencias.db";

// Registro das camadas de Infraestrutura e Aplicação
builder.Services.AddInfra(connectionString);
builder.Services.AddAplicacao();

// Configuração do manipulador de exceções
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();



// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Inicialização automática do banco SQLite com dados iniciais (Conta A: 100000, Conta B: 50000)
app.Services.InicializarBanco();

app.UseExceptionHandler();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

// -----------------------------------------------------------------------------
// Endpoints (Minimal APIs)
// -----------------------------------------------------------------------------

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

app.Run();
