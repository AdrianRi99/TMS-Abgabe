using FluentAssertions;
using TMS.Addresses.Application.DTOs.AddressDTO;
using TMS.Addresses.Application.Validators;

namespace TMS.Addresses.Tests.Validators;

public class AddressSearchRequestValidatorTests
{
    private readonly AddressSearchRequestValidator _validator = new();

    [Theory]
    [InlineData(1, 20, true)]
    [InlineData(0, 20, false)]
    [InlineData(1, 0, false)]
    [InlineData(1, 101, false)]
    public void Validate_ChecksPagingBounds(int page, int pageSize, bool expectedValid)
    {
        var result = _validator.Validate(new AddressSearchRequest(Page: page, PageSize: pageSize));

        result.IsValid.Should().Be(expectedValid);
    }

    [Theory]
    [InlineData(null, true)]
    [InlineData("Mustermann", true)]
    [InlineData("", true)]
    public void Validate_WithName_HasNoErrors(string? name, bool expectedValid)
    {
        _validator.Validate(new AddressSearchRequest(Name: name)).IsValid.Should().Be(expectedValid);
    }

    [Fact]
    public void Validate_WithTooLongName_Fails()
    {
        var result = _validator.Validate(new AddressSearchRequest(Name: new string('x', 201)));

        result.Errors.Should().Contain(e => e.PropertyName == nameof(AddressSearchRequest.Name));
    }
}
