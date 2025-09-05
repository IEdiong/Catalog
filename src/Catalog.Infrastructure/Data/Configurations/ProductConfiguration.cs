using Catalog.Domain.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Infrastructure.Data.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(p => p.Id);
        
        builder.Property(p => p.Id)
            .ValueGeneratedNever();

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(Product.MaxNameLength);

        builder.Property(p => p.Description)
            .IsRequired()
            .HasMaxLength(Product.MaxDescriptionLength);

        builder.Property(p => p.Price)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(p => p.StockQuantity)
            .IsRequired();

        builder.Property(p => p.IsActive)
            .IsRequired();

        builder.Property(p => p.CreatedAt)
            .IsRequired();

        builder.Property(p => p.UpdatedAt);

        builder.Property(p => p.Version)
            .IsRequired()
            .IsConcurrencyToken();

        builder.HasIndex(p => p.Name);
        builder.HasIndex(p => p.IsActive);
        
        // Ignore domain events - they're not persisted
        builder.Ignore(p => p.DomainEvents);
    }
}