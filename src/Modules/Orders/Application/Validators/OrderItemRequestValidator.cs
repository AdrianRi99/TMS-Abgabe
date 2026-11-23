using FluentValidation;
using TMS.Orders.Application.DTOs;

namespace TMS.Orders.Application.Validators;

public class OrderItemRequestValidator : AbstractValidator<OrderItemRequest>
{
    public OrderItemRequestValidator()
    {
        RuleFor(x => x.Description).NotEmpty().MaximumLength(500);
        RuleFor(x => x.Price).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Quantity).GreaterThanOrEqualTo(1);
    }
}
