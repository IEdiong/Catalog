using Catalog.Application.Common.Interfaces;
using Catalog.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Storage;

namespace Catalog.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly CatalogDbContext _context;
    private IDbContextTransaction? _transaction;
    private bool _disposed = false;

    public UnitOfWork(CatalogDbContext context)
    {
        _context = context;
        Products = new ProductRepository(_context);
        Orders = new OrderRepository(_context);
    }

    public IProductRepository Products { get; }
    public IOrderRepository Orders { get; }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
            throw new InvalidOperationException("Transaction already started");

        _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction == null)
            throw new InvalidOperationException("No transaction started");

        try
        {
            await SaveChangesAsync(cancellationToken);
            await _transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await RollbackTransactionAsync(cancellationToken);
            throw;
        }
        finally
        {
            _transaction?.Dispose();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction == null)
            throw new InvalidOperationException("No transaction started");

        try
        {
            await _transaction.RollbackAsync(cancellationToken);
        }
        finally
        {
            _transaction?.Dispose();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            _transaction?.Dispose();
            _context.Dispose();
            _disposed = true;
        }
    }
}