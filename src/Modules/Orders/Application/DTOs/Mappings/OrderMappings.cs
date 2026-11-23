using TMS.Orders.Domain.Entities;

namespace TMS.Orders.Application.DTOs.Mappings;

public static class OrderMappings
{
    public static OrderItemDto ToDto(this OrderItem i) =>
        new(i.Id, i.Description, i.Price, i.Quantity);

    public static OrderDto ToDto(this Order o) =>
        new(o.Id, o.AddressId, o.CustomerName, o.OrderDate, o.Status,
            o.Items.Select(i => i.ToDto()).ToList(),
            o.Items.Sum(i => i.Price * i.Quantity));

    public static OrderItem ToEntity(this OrderItemRequest r) =>
        new(r.Description, r.Price, r.Quantity);
}
