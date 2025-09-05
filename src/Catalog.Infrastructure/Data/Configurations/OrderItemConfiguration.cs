using Catalog.Domain.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Infrastructure.Data.Configurations;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.HasKey(oi => oi.Id);
        
        builder.Property(oi => oi.Id)
            .ValueGeneratedNever();

        builder.Property(oi => oi.ProductId)
            .IsRequired();

        builder.Property(oi => oi.ProductName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(oi => oi.Price)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(oi => oi.Quantity)
            .IsRequired();

        builder.Ignore(oi => oi.LineTotal);

        builder.HasIndex(oi => oi.ProductId);
    }
}