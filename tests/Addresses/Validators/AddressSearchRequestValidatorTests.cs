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
}