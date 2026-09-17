using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EcommerceOrders.Domain.Entities;

namespace EcommerceOrders.Infrastructure.Configurations;

public class ItemPedidoConfiguration : IEntityTypeConfiguration<ItemPedido>
{
    public void Configure(EntityTypeBuilder<ItemPedido> builder)
    {
        builder.ToTable("ItensPedido");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.PedidoId)
            .IsRequired();

        builder.Property(i => i.ProdutoId)
            .IsRequired();

        builder.Property(i => i.NomeProduto)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(i => i.PrecoUnitario)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(i => i.Quantidade)
            .IsRequired();

        builder.Ignore(i => i.ValorTotal);
    }
}