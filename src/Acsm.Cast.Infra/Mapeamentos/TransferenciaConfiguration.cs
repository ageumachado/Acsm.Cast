using Acsm.Cast.Dominio.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Acsm.Cast.Infra.Mapeamentos;

public class TransferenciaConfiguration : IEntityTypeConfiguration<Transferencia>
{
    public void Configure(EntityTypeBuilder<Transferencia> builder)
    {
        builder.ToTable("Transferencias");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.OrigemId)
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(t => t.DestinoId)
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(t => t.ValorCentavos)
            .IsRequired();

        builder.Property(t => t.CriadoEmUtc)
            .IsRequired();

        // Índices para otimizar a busca por movimentações de contas
        builder.HasIndex(t => t.OrigemId);
        builder.HasIndex(t => t.DestinoId);
    }
}