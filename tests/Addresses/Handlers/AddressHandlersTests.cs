using FluentAssertions;
using NSubstitute;
using TMS.Addresses.Application.DTOs.AddressDTO;
using TMS.Addresses.Application.Handlers;
using TMS.Addresses.Application.Interfaces;
using TMS.Addresses.Domain.Entities;

namespace TMS.Addresses.Tests.Handlers;

public class AddressHandlersTests
{
    private readonly IAddressRepository _repository;
    private readonly AddressHandlers _handlers;

    public AddressHandlersTests()
    {
        _repository = Substitute.For<IAddressRepository>();
        _handlers = new AddressHandlers(_repository);
    }

    private static Address CreateTestAddress()
    {
        var country = new Country("Österreich", "AUT");
        var city = new City("Wien", "1010", 1, country);
        return new Address("Hauptstraße", "12", 1, null, city);
    }

    [Fact]
    public async Task GetByIdAsync_WithExistingId_ReturnsAddressDto()
    {
        var address = CreateTestAddress();
        _repository.GetByIdAsync(address.Id).Returns(address);

        var result = await _handlers.GetByIdAsync(address.Id);

        result.Should().NotBeNull();
        result!.Street.Should().Be("Hauptstraße");
        result.CityName.Should().Be("Wien");
        result.CountryName.Should().Be("Österreich");
    }

    [Fact]
    public async Task GetByIdAsync_WithNonExistingId_ReturnsNull()
    {
        _repository.GetByIdAsync(Arg.Any<Guid>()).Returns((Address?)null);

        var result = await _handlers.GetByIdAsync(Guid.NewGuid());

        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateAsync_WithValidRequest_ReturnsCreatedAddressDto()
    {
        var request = new CreateAddressRequest("Neue Straße", "5", null, 1);
        var country = new Country("Österreich", "AUT");
        var city = new City("Wien", "1010", 1, country);

        _repository
            .When(r => r.AddAsync(Arg.Any<Address>()))
            .Do(_ => { });

        _repository
            .GetByIdAsync(Arg.Any<Guid>())
            .Returns(ci => new Address(
                request.Street,
                request.HouseNumber,
                request.CityId,
                request.Supplement,
                city));

        var result = await _handlers.CreateAsync(request);

        result.Should().NotBeNull();
        result.Street.Should().Be("Neue Straße");
        await _repository.Received(1).AddAsync(Arg.Any<Address>());
    }

    [Fact]
    public async Task DeleteAsync_WithNonExistingId_ReturnsFalse()
    {
        _repository.GetByIdAsync(Arg.Any<Guid>()).Returns((Address?)null);

        var result = await _handlers.DeleteAsync(Guid.NewGuid());

        result.Should().BeFalse();
        await _repository.DidNotReceive().DeleteAsync(Arg.Any<Guid>());
    }

    [Fact]
    public async Task DeleteAsync_WithExistingId_ReturnsTrue()
    {
        var address = CreateTestAddress();
        _repository.GetByIdAsync(address.Id).Returns(address);

        var result = await _handlers.DeleteAsync(address.Id);

        result.Should().BeTrue();
        await _repository.Received(1).DeleteAsync(address.Id);
    }
}