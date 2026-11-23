namespace TMS.Orders.Application.DTOs;

public record OrderItemDto(Guid Id, string Description, decimal Price, int Quantity);

public record OrderDto(
    Guid Id, Guid AddressId, string CustomerName, DateTime OrderDate,
    string Status, IReadOnlyList<OrderItemDto> Items, decimal TotalPrice);

public record OrderItemRequest(string Description, decimal Price, int Quantity);

public record CreateOrderRequest(Guid AddressId, IReadOnlyList<OrderItemRequest> Items);

public record UpdateOrderRequest(string Status, IReadOnlyList<OrderItemRequest> Items);

public record OrderSearchRequest(
    int Page = 1, int PageSize = 20, string SortBy = "orderdate", string SortDirection = "desc");
