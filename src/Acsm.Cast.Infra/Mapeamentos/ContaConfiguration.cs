using Acsm.Cast.Dominio.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Acsm.Cast.Infra.Mapeamentos;

public class ContaConfiguration : IEntityTypeConfiguration<Conta>
{
    public void Configure(EntityTypeBuilder<Conta> builder)
    {
        builder.ToTable("Contas");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(c => c.SaldoCentavos)
            .IsRequired();
    }
}
