using FluentValidation;
using TMS.Orders.Application.DTOs;

namespace TMS.Orders.Application.Validators;

public class CreateOrderValidator : AbstractValidator<CreateOrderRequest>
{
    public CreateOrderValidator()
    {
        RuleFor(x => x.AddressId).NotEmpty();
        RuleFor(x => x.Items).NotEmpty();
        RuleForEach(x => x.Items).SetValidator(new OrderItemRequestValidator());
    }
}
