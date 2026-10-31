using Acsm.Cast.Api.Middlewares;
using Acsm.Cast.Infra;
using Scalar.AspNetCore;
using Acsm.Cast.Aplicacao;
using Acsm.Cast.Api.Endpoints;

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

app.MapearEndpoints();

app.Run();
