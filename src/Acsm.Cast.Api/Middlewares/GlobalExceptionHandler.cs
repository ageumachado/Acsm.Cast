using Acsm.Cast.Dominio.Excecoes;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Acsm.Cast.Api.Middlewares;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (statusCode, title) = exception switch
        {
            RegraNegocioException => (StatusCodes.Status400BadRequest, "Requisição Inválida"),
            BadHttpRequestException => (StatusCodes.Status400BadRequest, "Erro no Corpo da Requisição"),
            ContaNaoEncontradaException => (StatusCodes.Status404NotFound, "Recurso Não Encontrado"),
            SaldoInsuficienteException => (StatusCodes.Status409Conflict, "Conflito de Saldo"),
            _ => (StatusCodes.Status500InternalServerError, "Erro Interno do Servidor")
        };

        if (statusCode == StatusCodes.Status500InternalServerError)
        {
            logger.LogError(exception, "Erro não tratado: {Mensagem}", exception.Message);
        }

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = exception.Message,
            Instance = httpContext.Request.Path
        };

        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}
