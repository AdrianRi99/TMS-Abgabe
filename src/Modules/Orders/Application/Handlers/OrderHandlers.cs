using Microsoft.Extensions.Logging;
using TMS.Orders.Application.Common;
using TMS.Orders.Application.DTOs;
using TMS.Orders.Application.DTOs.Mappings;
using TMS.Orders.Application.Interfaces;
using TMS.Orders.Domain.Entities;

namespace TMS.Orders.Application.Handlers;

public class OrderHandlers
{
    private readonly IOrderRepository _orders;
    private readonly ICustomerLookup _customers;
    private readonly ILogger<OrderHandlers> _logger;

    public OrderHandlers(IOrderRepository orders, ICustomerLookup customers, ILogger<OrderHandlers> logger)
    {
        _orders = orders;
        _customers = customers;
        _logger = logger;
    }

    public async Task<OrderDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        _logger.LogDebug("Fetching order {OrderId}", id);
        var order = await _orders.GetByIdAsync(id, ct);
        if (order is null)
            _logger.LogWarning("Order {OrderId} not found", id);
        return order?.ToDto();
    }

    public async Task<PagedResult<OrderDto>> SearchAsync(
        OrderSearchRequest request, CancellationToken ct = default)
    {
        _logger.LogDebug(
            "Searching orders: page={Page} pageSize={PageSize} sortBy={SortBy} sortDirection={SortDirection}",
            request.Page, request.PageSize, request.SortBy, request.SortDirection);

        var (items, totalCount) = await _orders.SearchAsync(
            request.Page, request.PageSize, request.SortBy, request.SortDirection, ct);

        return new PagedResult<OrderDto>(
            items.Select(o => o.ToDto()).ToList(), totalCount, request.Page, request.PageSize);
    }

    /// <returns>The created order, or null if the referenced address does not exist.</returns>
    public async Task<OrderDto?> CreateAsync(CreateOrderRequest request, CancellationToken ct = default)
    {
        var customerName = await _customers.GetCustomerNameAsync(request.AddressId, ct);
        if (customerName is null)
        {
            _logger.LogWarning("Cannot create order: address {AddressId} not found", request.AddressId);
            return null;
        }

        var order = new Order(request.AddressId, customerName, request.Items.Select(i => i.ToEntity()));
        await _orders.AddAsync(order, ct);
        _logger.LogInformation("Created order {OrderId} for address {AddressId}", order.Id, order.AddressId);
        return order.ToDto();
    }

    public async Task<OrderDto?> UpdateAsync(Guid id, UpdateOrderRequest request, CancellationToken ct = default)
    {
        var order = await _orders.GetByIdAsync(id, ct);
        if (order is null)
        {
            _logger.LogWarning("Cannot update order {OrderId}: not found", id);
            return null;
        }

        order.Update(request.Status, request.Items.Select(i => i.ToEntity()));
        await _orders.UpdateAsync(order, ct);
        _logger.LogInformation("Updated order {OrderId}", id);
        return order.ToDto();
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var order = await _orders.GetByIdAsync(id, ct);
        if (order is null)
        {
            _logger.LogWarning("Cannot delete order {OrderId}: not found", id);
            return false;
        }

        await _orders.DeleteAsync(id, ct);
        _logger.LogInformation("Deleted order {OrderId}", id);
        return true;
    }
}
