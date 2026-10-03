using FluentAssertions;
using TMS.Addresses.Application.DTOs.AddressDTO;
using TMS.Addresses.Application.Validators;

namespace TMS.Addresses.Tests.Validators;

public class CreateAddressValidatorTests
{
    private readonly CreateAddressValidator _validator = new();

    private static CreateAddressRequest ValidRequest() =>
        new("Hauptstraße", "12", null, "Wien", "1010", "Österreich");

    public static TheoryData<CreateAddressRequest, string> InvalidRequests => new()
    {
        { ValidRequest() with { Street = "  " }, nameof(CreateAddressRequest.Street) },
        { ValidRequest() with { HouseNumber = "" }, nameof(CreateAddressRequest.HouseNumber) },
        { ValidRequest() with { Supplement = new string('x', 101) }, nameof(CreateAddressRequest.Supplement) },
        { ValidRequest() with { CityName = "" }, nameof(CreateAddressRequest.CityName) },
        { ValidRequest() with { ZipCode = new string('1', 21) }, nameof(CreateAddressRequest.ZipCode) },
        { ValidRequest() with { CountryName = "" }, nameof(CreateAddressRequest.CountryName) },
    };

    [Fact]
    public void Validate_WithValidRequest_HasNoErrors()
    {
        _validator.Validate(ValidRequest()).IsValid.Should().BeTrue();
    }

    [Theory]
    [MemberData(nameof(InvalidRequests))]
    public void Validate_WithInvalidRequest_ReportsErrorForProperty(
        CreateAddressRequest request, string property)
    {
        var result = _validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == property);
    }
}