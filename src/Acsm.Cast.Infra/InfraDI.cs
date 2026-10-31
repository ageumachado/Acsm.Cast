using Acsm.Cast.Dominio.Contratos;
using Acsm.Cast.Infra.Contexto;
using Acsm.Cast.Infra.Repositorios;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Acsm.Cast.Infra;

public static class InfraDI
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddInfra(string connectionString = "Data Source=cast_transferencias.db")
        {
            services.AddDbContext<CastDbContext>(options =>
            {
                options.UseSqlite(connectionString);
            });

            services.AddScoped<IContaRepository, ContaRepository>();
            services.AddScoped<ITransferenciaRepository, TransferenciaRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }

    extension(IServiceProvider serviceProvider)
    {
        /// <summary>
        /// Garante que o banco SQLite e as tabelas com o Seed inicial sejam criados na inicialização limpa.
        /// </summary>
        public void InicializarBanco()
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<CastDbContext>();
            context.Database.EnsureCreated();
        }
    }
}
