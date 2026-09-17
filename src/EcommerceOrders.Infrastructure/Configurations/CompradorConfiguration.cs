using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EcommerceOrders.Domain.Entities;

namespace EcommerceOrders.Infrastructure.Configurations;

public class CompradorConfiguration : IEntityTypeConfiguration<Comprador>
{
    public void Configure(EntityTypeBuilder<Comprador> builder)
    {
        builder.ToTable("Compradores");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Nome)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(c => c.Email)
            .IsRequired()
            .HasMaxLength(150);
    }
}