using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EcommerceOrders.Domain.Entities;

namespace EcommerceOrders.Infrastructure.Configurations;

public class PedidoConfiguration : IEntityTypeConfiguration<Pedido>
{
    public void Configure(EntityTypeBuilder<Pedido> builder)
    {
        builder.ToTable("Pedidos");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.CompradorId)
            .IsRequired();

        builder.Property(p => p.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(p => p.DataCriacao)
            .IsRequired();

        builder.Property(p => p.DataAtualizacao);

        builder.Ignore(p => p.ValorTotalPedido);

        builder.HasMany(p => p.Itens)
            .WithOne()
            .HasForeignKey(i => i.PedidoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(p => p.Comprador)
            .WithMany()
            .HasForeignKey(p => p.CompradorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}