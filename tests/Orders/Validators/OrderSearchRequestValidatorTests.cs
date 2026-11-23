using FluentAssertions;
using TMS.Orders.Application.DTOs;
using TMS.Orders.Application.Validators;

namespace TMS.Orders.Tests.Validators;

public class OrderSearchRequestValidatorTests
{
    private readonly OrderSearchRequestValidator _validator = new();

    [Fact]
    public void Validate_WithDefaults_HasNoErrors()
    {
        _validator.Validate(new OrderSearchRequest()).IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_WithInvalidPage_Fails(int page)
    {
        _validator.Validate(new OrderSearchRequest(Page: page)).Errors
            .Should().Contain(e => e.PropertyName == nameof(OrderSearchRequest.Page));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(OrderSearchRequestValidator.MaxPageSize + 1)]
    public void Validate_WithInvalidPageSize_Fails(int pageSize)
    {
        _validator.Validate(new OrderSearchRequest(PageSize: pageSize)).Errors
            .Should().Contain(e => e.PropertyName == nameof(OrderSearchRequest.PageSize));
    }

    [Fact]
    public void Validate_WithMaxPageSize_HasNoErrors()
    {
        _validator.Validate(new OrderSearchRequest(PageSize: OrderSearchRequestValidator.MaxPageSize))
            .IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("orderdate")]
    [InlineData("CustomerName")]
    [InlineData("status")]
    public void Validate_WithAllowedSortBy_HasNoErrors(string sortBy)
    {
        _validator.Validate(new OrderSearchRequest(SortBy: sortBy)).IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithUnknownSortBy_Fails()
    {
        _validator.Validate(new OrderSearchRequest(SortBy: "totalprice")).Errors
            .Should().Contain(e => e.PropertyName == nameof(OrderSearchRequest.SortBy));
    }

    [Fact]
    public void Validate_WithUnknownSortDirection_Fails()
    {
        _validator.Validate(new OrderSearchRequest(SortDirection: "up")).Errors
            .Should().Contain(e => e.PropertyName == nameof(OrderSearchRequest.SortDirection));
    }
}
