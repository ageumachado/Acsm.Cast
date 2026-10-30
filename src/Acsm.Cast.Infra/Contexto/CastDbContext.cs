using Acsm.Cast.Dominio.Entidades;
using Microsoft.EntityFrameworkCore;

namespace Acsm.Cast.Infra.Contexto;

public class CastDbContext : DbContext
{
    public CastDbContext(DbContextOptions<CastDbContext> options) : base(options) { }

    public DbSet<Conta> Contas => Set<Conta>();
    public DbSet<Transferencia> Transferencias => Set<Transferencia>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CastDbContext).Assembly);

        // Seed das contas padrão requeridas pelo teste
        modelBuilder.Entity<Conta>().HasData(
            new Conta("A", 100000),
            new Conta("B", 50000)
        );
    }
}
