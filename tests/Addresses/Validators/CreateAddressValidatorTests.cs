using FluentAssertions;
using TMS.Addresses.Application.DTOs.AddressDTO;
using TMS.Addresses.Application.Validators;

namespace TMS.Addresses.Tests.Validators;

public class CreateAddressValidatorTests
{
    private readonly CreateAddressValidator _validator = new();

    [Fact]
    public void Validate_WithValidRequest_ShouldNotHaveErrors()
    {
        var request = new CreateAddressRequest("Hauptstraße", "12", null, 1);

        var result = _validator.Validate(request);

        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_WithEmptyStreet_ShouldHaveError(string street)
    {
        var request = new CreateAddressRequest(street, "12", null, 1);

        var result = _validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Street");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_WithEmptyHouseNumber_ShouldHaveError(string houseNumber)
    {
        var request = new CreateAddressRequest("Hauptstraße", houseNumber, null, 1);

        var result = _validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "HouseNumber");
    }

    [Fact]
    public void Validate_WithInvalidCityId_ShouldHaveError()
    {
        var request = new CreateAddressRequest("Hauptstraße", "12", null, 0);

        var result = _validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "CityId");
    }

    [Fact]
    public void Validate_WithSupplementTooLong_ShouldHaveError()
    {
        var request = new CreateAddressRequest("Hauptstraße", "12", new string('x', 101), 1);

        var result = _validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Supplement");
    }
}