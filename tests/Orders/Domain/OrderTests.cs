using FluentAssertions;
using TMS.Orders.Domain.Entities;

namespace TMS.Orders.Tests.Domain;

public class OrderTests
{
    private static OrderItem[] Items(params string[] descriptions) =>
        descriptions.Select(d => new OrderItem(d, 1m, 1)).ToArray();

    [Fact]
    public void Constructor_AssignsPositionsInEnteredOrder()
    {
        var order = new Order(Guid.NewGuid(), "Max", Items("A", "B", "C"));

        order.Items.Select(i => i.Description).Should().Equal("A", "B", "C");
        order.Items.Select(i => i.Position).Should().Equal(0, 1, 2);
    }

    [Fact]
    public void Update_ReplacesItemsAndRestartsPositionsAtZero()
    {
        var order = new Order(Guid.NewGuid(), "Max", Items("A", "B", "C"));

        order.Update("Abgeschlossen", Items("X", "Y"));

        order.Status.Should().Be("Abgeschlossen");
        order.Items.Select(i => i.Description).Should().Equal("X", "Y");
        order.Items.Select(i => i.Position).Should().Equal(0, 1);
    }

    [Fact]
    public void Constructor_KeepsSnapshotAndSetsOpenStatus()
    {
        var addressId = Guid.NewGuid();

        var order = new Order(addressId, "Erika", Items("A"));

        order.AddressId.Should().Be(addressId);
        order.CustomerName.Should().Be("Erika");
        order.Status.Should().Be("Offen");
        order.OrderDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }
}
