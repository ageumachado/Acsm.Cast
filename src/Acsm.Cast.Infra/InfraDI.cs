using Acsm.Cast.Dominio.Contratos;
using Acsm.Cast.Infra.Contexto;
using Acsm.Cast.Infra.Repositorios;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Acsm.Cast.Infra;

public static class InfraDI
{
    public static IServiceCollection AddInfra(this IServiceCollection services, string connectionString = "Data Source=cast_transferencias.db")
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

    /// <summary>
    /// Garante que o banco SQLite e as tabelas com o Seed inicial sejam criados na inicialização limpa.
    /// </summary>
    public static void InicializarBanco(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<CastDbContext>();
        context.Database.EnsureCreated();
    }
}
