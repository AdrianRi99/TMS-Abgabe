using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using TMS.Orders.Application.DTOs;
using TMS.Orders.Application.Handlers;
using TMS.Orders.Application.Interfaces;
using TMS.Orders.Domain;
using TMS.Orders.Domain.Entities;

namespace TMS.Orders.Tests.Handlers;

public class OrderHandlersTests
{
    private readonly IOrderRepository _orders = Substitute.For<IOrderRepository>();
    private readonly ICustomerLookup _customers = Substitute.For<ICustomerLookup>();
    private readonly OrderHandlers _handlers;

    public OrderHandlersTests()
    {
        _handlers = new OrderHandlers(_orders, _customers, NullLogger<OrderHandlers>.Instance);
    }

    private static Order CreateTestOrder(Guid? addressId = null, string customerName = "Max Mustermann") =>
        new(addressId ?? Guid.NewGuid(), customerName,
        [
            new OrderItem("Schrauben", 2.50m, 4),
            new OrderItem("Muttern", 1m, 10)
        ]);

    // ---- GetById ----

    [Fact]
    public async Task GetByIdAsync_WhenExists_ReturnsDtoWithTotalPrice()
    {
        var order = CreateTestOrder();
        _orders.GetByIdAsync(order.Id, Arg.Any<CancellationToken>()).Returns(order);

        var result = await _handlers.GetByIdAsync(order.Id);

        result.Should().NotBeNull();
        result!.Id.Should().Be(order.Id);
        result.CustomerName.Should().Be("Max Mustermann");
        result.Items.Should().HaveCount(2);
        result.TotalPrice.Should().Be(20m); // 2.50*4 + 1*10
    }

    [Fact]
    public async Task GetByIdAsync_WhenMissing_ReturnsNull()
    {
        _orders.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((Order?)null);

        var result = await _handlers.GetByIdAsync(Guid.NewGuid());

        result.Should().BeNull();
    }

    // ---- Search ----

    [Fact]
    public async Task SearchAsync_PassesParametersAndMapsResult()
    {
        var order = CreateTestOrder();
        _orders.SearchAsync(2, 10, "status", "asc", Arg.Any<CancellationToken>())
            .Returns(((IReadOnlyList<Order>)[order], 11));

        var result = await _handlers.SearchAsync(new OrderSearchRequest(2, 10, "status", "asc"));

        result.Items.Should().ContainSingle().Which.Id.Should().Be(order.Id);
        result.TotalCount.Should().Be(11);
        result.Page.Should().Be(2);
        result.PageSize.Should().Be(10);
        result.TotalPages.Should().Be(2);
    }

    [Fact]
    public async Task SearchAsync_WhenEmpty_ReturnsEmptyPage()
    {
        _orders.SearchAsync(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<string>(), Arg.Any<string>(),
                Arg.Any<CancellationToken>())
            .Returns(((IReadOnlyList<Order>)[], 0));

        var result = await _handlers.SearchAsync(new OrderSearchRequest());

        result.Items.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
    }

    // ---- Create ----

    [Fact]
    public async Task CreateAsync_WhenAddressExists_SnapshotsCustomerNameAndPersists()
    {
        var addressId = Guid.NewGuid();
        _customers.GetCustomerNameAsync(addressId, Arg.Any<CancellationToken>()).Returns("Erika Muster");
        var request = new CreateOrderRequest(addressId,
            [new OrderItemRequest("Bohrer", 19.99m, 2), new OrderItemRequest("Dübel", 0.10m, 50)]);

        var result = await _handlers.CreateAsync(request);

        result.Should().NotBeNull();
        result!.AddressId.Should().Be(addressId);
        result.CustomerName.Should().Be("Erika Muster");
        result.Status.Should().Be(OrderStatus.Open);
        result.OrderDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        result.Items.Should().HaveCount(2);
        result.TotalPrice.Should().Be(19.99m * 2 + 0.10m * 50);
        await _orders.Received(1).AddAsync(
            Arg.Is<Order>(o => o.AddressId == addressId && o.CustomerName == "Erika Muster"),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateAsync_WhenAddressUnknown_ReturnsNullAndDoesNotPersist()
    {
        _customers.GetCustomerNameAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((string?)null);

        var result = await _handlers.CreateAsync(
            new CreateOrderRequest(Guid.NewGuid(), [new OrderItemRequest("X", 1m, 1)]));

        result.Should().BeNull();
        await _orders.DidNotReceive().AddAsync(Arg.Any<Order>(), Arg.Any<CancellationToken>());
    }

    // ---- Update ----

    [Fact]
    public async Task UpdateAsync_WhenExists_ChangesStatusAndReplacesItems_KeepsSnapshot()
    {
        var order = CreateTestOrder(customerName: "Originalname");
        var originalAddressId = order.AddressId;
        var originalDate = order.OrderDate;
        _orders.GetByIdAsync(order.Id, Arg.Any<CancellationToken>()).Returns(order);

        var result = await _handlers.UpdateAsync(order.Id,
            new UpdateOrderRequest(OrderStatus.Completed, [new OrderItemRequest("Neu", 7m, 3)]));

        result.Should().NotBeNull();
        result!.Status.Should().Be(OrderStatus.Completed);
        result.Items.Should().ContainSingle().Which.Description.Should().Be("Neu");
        result.TotalPrice.Should().Be(21m);
        result.CustomerName.Should().Be("Originalname");
        result.AddressId.Should().Be(originalAddressId);
        result.OrderDate.Should().Be(originalDate);
        await _orders.Received(1).UpdateAsync(order, Arg.Any<CancellationToken>());
        await _customers.DidNotReceive().GetCustomerNameAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateAsync_WhenMissing_ReturnsNullAndDoesNotPersist()
    {
        _orders.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((Order?)null);

        var result = await _handlers.UpdateAsync(Guid.NewGuid(),
            new UpdateOrderRequest(OrderStatus.Cancelled, [new OrderItemRequest("X", 1m, 1)]));

        result.Should().BeNull();
        await _orders.DidNotReceive().UpdateAsync(Arg.Any<Order>(), Arg.Any<CancellationToken>());
    }

    // ---- Delete ----

    [Fact]
    public async Task DeleteAsync_WhenExists_DeletesAndReturnsTrue()
    {
        var order = CreateTestOrder();
        _orders.GetByIdAsync(order.Id, Arg.Any<CancellationToken>()).Returns(order);

        var result = await _handlers.DeleteAsync(order.Id);

        result.Should().BeTrue();
        await _orders.Received(1).DeleteAsync(order.Id, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteAsync_WhenMissing_ReturnsFalseAndDoesNotDelete()
    {
        _orders.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((Order?)null);

        var result = await _handlers.DeleteAsync(Guid.NewGuid());

        result.Should().BeFalse();
        await _orders.DidNotReceive().DeleteAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }
}
