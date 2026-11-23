namespace TMS.Orders.Domain.Entities;

public class Order
{
    private readonly List<OrderItem> _items = [];

    public Guid Id { get; private set; }

    /// <summary>Reference to an address of the Addresses module. No DB foreign key by design.</summary>
    public Guid AddressId { get; private set; }

    /// <summary>Snapshot of Address.Name at order creation; never reloaded afterwards.</summary>
    public string CustomerName { get; private set; } = string.Empty;

    public DateTime OrderDate { get; private set; }
    public string Status { get; private set; } = OrderStatus.Open;
    public IReadOnlyCollection<OrderItem> Items => _items;

    private Order() { }

    public Order(Guid addressId, string customerName, IEnumerable<OrderItem> items)
    {
        Id = Guid.NewGuid();
        AddressId = addressId;
        CustomerName = customerName;
        OrderDate = DateTime.UtcNow;
        Status = OrderStatus.Open;
        _items.AddRange(items);
    }

    public void Update(string status, IEnumerable<OrderItem> items)
    {
        Status = status;
        _items.Clear();
        _items.AddRange(items);
    }
}
