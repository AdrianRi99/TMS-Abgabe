namespace TMS.Orders.Domain.Entities;

public class OrderItem
{
    public Guid Id { get; private set; }
    public Guid OrderId { get; private set; }
    public string Description { get; private set; } = string.Empty;
    public decimal Price { get; private set; }
    public int Quantity { get; private set; }

    private OrderItem() { }

    public OrderItem(string description, decimal price, int quantity)
    {
        Id = Guid.NewGuid();
        Description = description;
        Price = price;
        Quantity = quantity;
    }
}
