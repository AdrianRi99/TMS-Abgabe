using FluentValidation;
using TMS.Orders.Application.DTOs;

namespace TMS.Orders.Application.Validators;

public class OrderSearchRequestValidator : AbstractValidator<OrderSearchRequest>
{
    public const int MaxPageSize = 100;

    private static readonly string[] ValidSortFields = ["orderdate", "customername", "status"];

    public OrderSearchRequestValidator()
    {
        RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, MaxPageSize);
        RuleFor(x => x.SortBy)
            .Must(v => ValidSortFields.Contains(v.ToLowerInvariant()))
            .When(x => !string.IsNullOrEmpty(x.SortBy))
            .WithMessage($"Erlaubte Werte: {string.Join(", ", ValidSortFields)}");
        RuleFor(x => x.SortDirection)
            .Must(v => v is "asc" or "desc")
            .When(x => !string.IsNullOrEmpty(x.SortDirection))
            .WithMessage("Erlaubte Werte: asc, desc");
    }
}
