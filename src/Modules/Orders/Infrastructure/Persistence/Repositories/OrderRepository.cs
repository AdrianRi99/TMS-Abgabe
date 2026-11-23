using Microsoft.EntityFrameworkCore;
using TMS.Orders.Application.Interfaces;
using TMS.Orders.Domain.Entities;

namespace TMS.Orders.Infrastructure.Persistence.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly OrdersDbContext _context;

    public OrderRepository(OrdersDbContext context)
    {
        _context = context;
    }

    public async Task<Order?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.Orders
            .AsNoTracking()
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == id, ct);
    }

    public async Task<(IReadOnlyList<Order> Items, int TotalCount)> SearchAsync(
        int page, int pageSize, string sortBy, string sortDirection, CancellationToken ct = default)
    {
        var query = _context.Orders
            .AsNoTracking()
            .Include(o => o.Items)
            .AsSplitQuery()
            .AsQueryable();

        var totalCount = await query.CountAsync(ct);

        var desc = sortDirection.Equals("desc", StringComparison.OrdinalIgnoreCase);

        // Id as tie-breaker keeps paging deterministic.
        query = sortBy.ToLowerInvariant() switch
        {
            "customername" => desc
                ? query.OrderByDescending(o => o.CustomerName).ThenBy(o => o.Id)
                : query.OrderBy(o => o.CustomerName).ThenBy(o => o.Id),
            "status" => desc
                ? query.OrderByDescending(o => o.Status).ThenBy(o => o.Id)
                : query.OrderBy(o => o.Status).ThenBy(o => o.Id),
            _ => desc
                ? query.OrderByDescending(o => o.OrderDate).ThenBy(o => o.Id)
                : query.OrderBy(o => o.OrderDate).ThenBy(o => o.Id)
        };

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, totalCount);
    }

    public async Task AddAsync(Order order, CancellationToken ct = default)
    {
        await _context.Orders.AddAsync(order, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Order order, CancellationToken ct = default)
    {
        var tracked = await _context.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == order.Id, ct)
            ?? throw new InvalidOperationException($"Order {order.Id} not found.");

        // AddressId, CustomerName and OrderDate are immutable; only status and items change.
        // Old items are orphaned (and deleted by cascade) and replaced by the new ones.
        tracked.Update(order.Status, order.Items);
        await _context.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var order = await _context.Orders.FindAsync([id], ct);
        if (order is null) return;

        _context.Orders.Remove(order); // items are removed via cascade
        await _context.SaveChangesAsync(ct);
    }
}
