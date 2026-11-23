using FluentAssertions;
using TMS.Orders.Application.DTOs;
using TMS.Orders.Application.Validators;

namespace TMS.Orders.Tests.Validators;

public class CreateOrderValidatorTests
{
    private readonly CreateOrderValidator _validator = new();

    private static OrderItemRequest ValidItem() => new("Schrauben", 1.5m, 2);

    private static CreateOrderRequest ValidRequest() =>
        new(Guid.NewGuid(), [ValidItem()]);

    public static TheoryData<CreateOrderRequest, string> InvalidRequests => new()
    {
        { ValidRequest() with { AddressId = Guid.Empty }, nameof(CreateOrderRequest.AddressId) },
        { ValidRequest() with { Items = [] }, nameof(CreateOrderRequest.Items) },
        { ValidRequest() with { Items = [ValidItem() with { Description = "  " }] }, "Items[0].Description" },
        { ValidRequest() with { Items = [ValidItem() with { Description = new string('x', 501) }] }, "Items[0].Description" },
        { ValidRequest() with { Items = [ValidItem() with { Price = -0.01m }] }, "Items[0].Price" },
        { ValidRequest() with { Items = [ValidItem() with { Quantity = 0 }] }, "Items[0].Quantity" },
        { ValidRequest() with { Items = [ValidItem(), ValidItem() with { Quantity = -1 }] }, "Items[1].Quantity" },
    };

    [Fact]
    public void Validate_WithValidRequest_HasNoErrors()
    {
        _validator.Validate(ValidRequest()).IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithBoundaryValues_HasNoErrors()
    {
        var request = ValidRequest() with
        {
            Items = [new OrderItemRequest(new string('x', 500), 0m, 1)]
        };

        _validator.Validate(request).IsValid.Should().BeTrue();
    }

    [Theory]
    [MemberData(nameof(InvalidRequests))]
    public void Validate_WithInvalidRequest_ReportsErrorForProperty(
        CreateOrderRequest request, string property)
    {
        var result = _validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == property);
    }
}
