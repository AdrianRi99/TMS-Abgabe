using FluentValidation;
using TMS.Addresses.Application.DTOs.AddressDTO;

namespace TMS.Addresses.Application.Validators;

public class AddressSearchRequestValidator : AbstractValidator<AddressSearchRequest>
{
    public const int MaxPageSize = 100;

    public AddressSearchRequestValidator()
    {
        RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, MaxPageSize);
        RuleFor(x => x.Street).MaximumLength(200);
        RuleFor(x => x.CityName).MaximumLength(100);
        RuleFor(x => x.CountryName).MaximumLength(100);
    }
}