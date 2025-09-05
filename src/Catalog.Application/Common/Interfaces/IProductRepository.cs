using Catalog.Domain.Products;

namespace Catalog.Application.Common.Interfaces;

public interface IProductRepository : IRepository<Product>
{
    Task<IEnumerable<Product>> GetActiveProductsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Product>> GetProductsByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);
    Task<IEnumerable<Product>> GetProductsByIdsWithLockAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);
    Task<Product?> GetByIdWithLockAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm, int skip = 0, int take = 10, CancellationToken cancellationToken = default);
}