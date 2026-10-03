using FluentValidation;
using TMS.Addresses.Application.DTOs.AddressDTO;

namespace TMS.Addresses.Application.Validators;

public class AddressSearchRequestValidator : AbstractValidator<AddressSearchRequest>
{
    public const int MaxPageSize = 100;

    private static readonly string[] ValidSortFields =
        ["street", "housenumber", "zipcode", "city", "country"];

    public AddressSearchRequestValidator()
    {
        RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, MaxPageSize);
        RuleFor(x => x.Street).MaximumLength(200).When(x => x.Street is not null);
        RuleFor(x => x.CityName).MaximumLength(100).When(x => x.CityName is not null);
        RuleFor(x => x.CountryName).MaximumLength(100).When(x => x.CountryName is not null);
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