using Catalog.Application.Common.Interfaces;
using Catalog.Domain.Products;
using Catalog.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Infrastructure.Repositories;

public class ProductRepository : Repository<Product>, IProductRepository
{
    public ProductRepository(CatalogDbContext context) : base(context) { }

    public async Task<IEnumerable<Product>> GetActiveProductsAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(p => p.IsActive)
            .OrderBy(p => p.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Product>> GetProductsByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(p => ids.Contains(p.Id))
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Product>> GetProductsByIdsWithLockAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
    {
        // For multiple products, we need to lock them in a consistent order to prevent deadlocks
        var orderedIds = ids.OrderBy(id => id).ToList();
        var products = new List<Product>();

        foreach (var id in orderedIds)
        {
            var product = await GetByIdWithLockAsync(id, cancellationToken);
            if (product != null)
                products.Add(product);
        }

        return products;
    }

    public async Task<Product?> GetByIdWithLockAsync(Guid id, CancellationToken cancellationToken = default)
    {
        // Use FromSqlRaw for row-level locking in PostgreSQL
        // This will acquire a FOR UPDATE lock on the specific row
        var sql = "SELECT * FROM \"Products\" WHERE \"Id\" = {0} FOR UPDATE";
        return await DbSet
            .FromSqlRaw(sql, id)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm, int skip = 0, int take = 10, CancellationToken cancellationToken = default)
    {
        var query = DbSet.Where(p => p.IsActive);

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(p => p.Name.Contains(searchTerm) || p.Description.Contains(searchTerm));
        }

        return await query
            .OrderBy(p => p.Name)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);
    }
}