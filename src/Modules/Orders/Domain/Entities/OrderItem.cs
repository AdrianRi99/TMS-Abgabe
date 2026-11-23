namespace TMS.Orders.Domain.Entities;

public class OrderItem
{
    public Guid Id { get; private set; }
    public Guid OrderId { get; private set; }
    public string Description { get; private set; } = string.Empty;
    public decimal Price { get; private set; }
    public int Quantity { get; private set; }

    /// <summary>Zero-based position within the order; keeps the entered order stable.</summary>
    public int Position { get; private set; }

    private OrderItem() { }

    public OrderItem(string description, decimal price, int quantity)
    {
        Id = Guid.NewGuid();
        Description = description;
        Price = price;
        Quantity = quantity;
    }

    internal void SetPosition(int position) => Position = position;
}
