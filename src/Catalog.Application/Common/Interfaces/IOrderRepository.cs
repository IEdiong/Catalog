using Catalog.Domain.Orders;

namespace Catalog.Application.Common.Interfaces;

public interface IOrderRepository : IRepository<Order>
{
    Task<IEnumerable<Order>> GetOrdersByCustomerEmailAsync(string customerEmail, CancellationToken cancellationToken = default);
    Task<IEnumerable<Order>> GetOrdersByStatusAsync(OrderStatus status, CancellationToken cancellationToken = default);
    Task<IEnumerable<Order>> GetOrdersWithItemsAsync(int skip = 0, int take = 10, CancellationToken cancellationToken = default);
    Task<Order?> GetByIdWithItemsAsync(Guid id, CancellationToken cancellationToken = default);
}