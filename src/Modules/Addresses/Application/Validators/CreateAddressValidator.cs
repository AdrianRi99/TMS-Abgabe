using FluentValidation;
using TMS.Addresses.Application.DTOs.AddressDTO;

namespace TMS.Addresses.Application.Validators;

public class CreateAddressValidator : AbstractValidator<CreateAddressRequest>
{
    public CreateAddressValidator()
    {
        RuleFor(x => x.Street).NotEmpty().MaximumLength(200);
        RuleFor(x => x.HouseNumber).NotEmpty().MaximumLength(10);
        RuleFor(x => x.Supplement).MaximumLength(100).When(x => x.Supplement is not null);
        RuleFor(x => x.CityName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.ZipCode).NotEmpty().MaximumLength(20);
        RuleFor(x => x.CountryName).NotEmpty().MaximumLength(100);
    }
}