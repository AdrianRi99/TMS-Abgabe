using FluentValidation;
using TMS.Orders.Application.DTOs;
using TMS.Orders.Domain;

namespace TMS.Orders.Application.Validators;

public class UpdateOrderValidator : AbstractValidator<UpdateOrderRequest>
{
    public UpdateOrderValidator()
    {
        RuleFor(x => x.Status)
            .Must(OrderStatus.IsValid)
            .WithMessage($"Erlaubte Werte: {string.Join(", ", OrderStatus.All)}");
        RuleFor(x => x.Items).NotEmpty();
        RuleForEach(x => x.Items).SetValidator(new OrderItemRequestValidator());
    }
}
