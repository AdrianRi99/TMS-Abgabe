using FluentAssertions;
using TMS.Orders.Application.DTOs;
using TMS.Orders.Application.Validators;
using TMS.Orders.Domain;

namespace TMS.Orders.Tests.Validators;

public class UpdateOrderValidatorTests
{
    private readonly UpdateOrderValidator _validator = new();

    private static OrderItemRequest ValidItem() => new("Schrauben", 1.5m, 2);

    private static UpdateOrderRequest ValidRequest() =>
        new(OrderStatus.Open, [ValidItem()]);

    public static TheoryData<UpdateOrderRequest, string> InvalidRequests => new()
    {
        { ValidRequest() with { Status = "Unbekannt" }, nameof(UpdateOrderRequest.Status) },
        { ValidRequest() with { Status = "" }, nameof(UpdateOrderRequest.Status) },
        { ValidRequest() with { Status = "offen" }, nameof(UpdateOrderRequest.Status) },
        { ValidRequest() with { Items = [] }, nameof(UpdateOrderRequest.Items) },
        { ValidRequest() with { Items = [ValidItem() with { Description = "" }] }, "Items[0].Description" },
        { ValidRequest() with { Items = [ValidItem() with { Price = -1m }] }, "Items[0].Price" },
        { ValidRequest() with { Items = [ValidItem() with { Quantity = 0 }] }, "Items[0].Quantity" },
    };

    [Fact]
    public void Validate_WithValidRequest_HasNoErrors()
    {
        _validator.Validate(ValidRequest()).IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData(OrderStatus.Open)]
    [InlineData(OrderStatus.Completed)]
    [InlineData(OrderStatus.Cancelled)]
    public void Validate_WithAllowedStatus_HasNoErrors(string status)
    {
        _validator.Validate(ValidRequest() with { Status = status }).IsValid.Should().BeTrue();
    }

    [Theory]
    [MemberData(nameof(InvalidRequests))]
    public void Validate_WithInvalidRequest_ReportsErrorForProperty(
        UpdateOrderRequest request, string property)
    {
        var result = _validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == property);
    }
}
