using FluentAssertions;
using NSubstitute;
using TMS.Addresses.Application.DTOs.AddressDTO;
using TMS.Addresses.Application.Handlers;
using TMS.Addresses.Application.Interfaces;
using TMS.Addresses.Domain.Entities;

namespace TMS.Addresses.Tests.Handlers;

public class AddressHandlersTests
{
    private readonly IAddressRepository _addresses = Substitute.For<IAddressRepository>();
    private readonly ICityRepository _cities = Substitute.For<ICityRepository>();
    private readonly ICountryRepository _countries = Substitute.For<ICountryRepository>();
    private readonly AddressHandlers _handlers;

    public AddressHandlersTests()
    {
        _handlers = new AddressHandlers(_addresses, _cities, _countries);
    }

    private static Address CreateTestAddress(string cityName = "Wien", string countryName = "Österreich")
    {
        var country = new Country(countryName, "AUT");
        var city = new City(cityName, "1010", 1, country);
        return new Address("Hauptstraße", "12", 1, null, city);
    }

    [Fact]
    public async Task GetByIdAsync_WithExistingId_ReturnsDto()
    {
        var address = CreateTestAddress();
        _addresses.GetByIdAsync(address.Id, Arg.Any<CancellationToken>()).Returns(address);

        var result = await _handlers.GetByIdAsync(address.Id);

        result.Should().NotBeNull();
        result!.Street.Should().Be("Hauptstraße");
        result.CityName.Should().Be("Wien");
        result.CountryName.Should().Be("Österreich");
    }

    [Fact]
    public async Task GetByIdAsync_WithUnknownId_ReturnsNull()
    {
        _addresses.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((Address?)null);

        var result = await _handlers.GetByIdAsync(Guid.NewGuid());

        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateAsync_WithUnknownCityAndCountry_CreatesBoth()
    {
        var request = new CreateAddressRequest("Neue Straße", "5", null, "Graz", "8010", "Österreich");
        _countries.FindByNameAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns((Country?)null);
        _cities.FindAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns((City?)null);
        _addresses.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(CreateTestAddress("Graz"));

        var result = await _handlers.CreateAsync(request);

        result.CityName.Should().Be("Graz");
        await _countries.Received(1).AddAsync(
            Arg.Is<Country>(c => c.Name == "Österreich"), Arg.Any<CancellationToken>());
        await _cities.Received(1).AddAsync(
            Arg.Is<City>(c => c.Name == "Graz" && c.ZipCode == "8010"), Arg.Any<CancellationToken>());
        await _addresses.Received(1).AddAsync(Arg.Any<Address>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateAsync_WithExistingCity_ReusesCityAndCountry()
    {
        var request = new CreateAddressRequest("Ringstraße", "1", null, "Wien", "1010", "Österreich");
        var country = new Country("Österreich", "AUT");
        _countries.FindByNameAsync("Österreich", Arg.Any<CancellationToken>()).Returns(country);
        _cities.FindAsync("Wien", "1010", Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns(new City("Wien", "1010", 1, country));
        _addresses.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(CreateTestAddress());

        await _handlers.CreateAsync(request);

        await _countries.DidNotReceive().AddAsync(Arg.Any<Country>(), Arg.Any<CancellationToken>());
        await _cities.DidNotReceive().AddAsync(Arg.Any<City>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateAsync_WithUnknownId_ReturnsNullAndDoesNotPersist()
    {
        _addresses.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((Address?)null);
        var request = new UpdateAddressRequest("X", "1", null, "Wien", "1010", "Österreich");

        var result = await _handlers.UpdateAsync(Guid.NewGuid(), request);

        result.Should().BeNull();
        await _addresses.DidNotReceive().UpdateAsync(Arg.Any<Address>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteAsync_WithUnknownId_ReturnsFalse()
    {
        _addresses.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((Address?)null);

        var result = await _handlers.DeleteAsync(Guid.NewGuid());

        result.Should().BeFalse();
        await _addresses.DidNotReceive().DeleteAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteAsync_WithExistingId_ReturnsTrue()
    {
        var address = CreateTestAddress();
        _addresses.GetByIdAsync(address.Id, Arg.Any<CancellationToken>()).Returns(address);

        var result = await _handlers.DeleteAsync(address.Id);

        result.Should().BeTrue();
        await _addresses.Received(1).DeleteAsync(address.Id, Arg.Any<CancellationToken>());
    }
}