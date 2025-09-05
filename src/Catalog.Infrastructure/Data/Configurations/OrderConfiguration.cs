using Catalog.Domain.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Infrastructure.Data.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(o => o.Id);
        
        builder.Property(o => o.Id)
            .ValueGeneratedNever();

        builder.Property(o => o.CustomerName)
            .IsRequired()
            .HasMaxLength(Order.MaxCustomerNameLength);

        builder.Property(o => o.CustomerEmail)
            .IsRequired()
            .HasMaxLength(Order.MaxCustomerEmailLength);

        builder.Property(o => o.Status)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(o => o.TotalAmount)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(o => o.OrderDate)
            .IsRequired();

        builder.Property(o => o.CompletedDate);

        builder.Property(o => o.Version)
            .IsRequired()
            .IsConcurrencyToken();

        builder.HasMany(o => o.Items)
            .WithOne()
            .HasForeignKey("OrderId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(o => o.CustomerEmail);
        builder.HasIndex(o => o.Status);
        builder.HasIndex(o => o.OrderDate);
        
        // Ignore domain events - they're not persisted
        builder.Ignore(o => o.DomainEvents);
    }
}