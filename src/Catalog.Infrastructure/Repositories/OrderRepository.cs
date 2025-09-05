using Catalog.Application.Common.Interfaces;
using Catalog.Domain.Orders;
using Catalog.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Infrastructure.Repositories;

public class OrderRepository : Repository<Order>, IOrderRepository
{
    public OrderRepository(CatalogDbContext context) : base(context) { }

    public async Task<IEnumerable<Order>> GetOrdersByCustomerEmailAsync(string customerEmail, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(o => o.Items)
            .Where(o => o.CustomerEmail == customerEmail.ToLowerInvariant())
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Order>> GetOrdersByStatusAsync(OrderStatus status, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(o => o.Items)
            .Where(o => o.Status == status)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Order>> GetOrdersWithItemsAsync(int skip = 0, int take = 10, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(o => o.Items)
            .OrderByDescending(o => o.OrderDate)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    public async Task<Order?> GetByIdWithItemsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
    }
}