using Acsm.Cast.Aplicacao.CasosDeUso;
using Microsoft.Extensions.DependencyInjection;

namespace Acsm.Cast.Aplicacao;

public static class InversaoControle
{
    public static IServiceCollection AddAplicacao(this IServiceCollection services)
    {
        services.AddScoped<IRealizarTransferenciaUseCase, RealizarTransferenciaUseCase>();
        services.AddScoped<IObterContaUseCase, ObterContaUseCase>();
        services.AddScoped<IListarMovimentacoesUseCase, ListarMovimentacoesUseCase>();

        return services;
    }
}
