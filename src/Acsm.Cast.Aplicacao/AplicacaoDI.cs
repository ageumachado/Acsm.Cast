using Acsm.Cast.Aplicacao.CasosDeUso;
using Microsoft.Extensions.DependencyInjection;

namespace Acsm.Cast.Aplicacao;

public static class AplicacaoDI
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddAplicacao()
        {
            services.AddScoped<IRealizarTransferenciaUseCase, RealizarTransferenciaUseCase>();
            services.AddScoped<IObterContaUseCase, ObterContaUseCase>();
            services.AddScoped<IListarMovimentacoesUseCase, ListarMovimentacoesUseCase>();

            return services;
        }
    }
}
