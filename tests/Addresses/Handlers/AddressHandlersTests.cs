using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
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
        _handlers = new AddressHandlers(
            _addresses,
            _cities,
            _countries,
            NullLogger<AddressHandlers>.Instance);
    }

    private static Address CreateTestAddress(
        string name = "Max Mustermann",
        string cityName = "Wien",
        string countryName = "Österreich")
    {
        var country = new Country(countryName, "AUT");
        var city = new City(cityName, "1010", 1, country);
        return new Address(name, "Hauptstraße", "12", 1, null, city);
    }

    [Fact]
    public async Task GetByIdAsync_WithExistingId_ReturnsDto()
    {
        var address = CreateTestAddress();
        _addresses.GetByIdAsync(address.Id, Arg.Any<CancellationToken>()).Returns(address);

        var result = await _handlers.GetByIdAsync(address.Id);

        result.Should().NotBeNull();
        result!.Name.Should().Be("Max Mustermann");
        result.Street.Should().Be("Hauptstraße");
        result.CityName.Should().Be("Wien");
        result.CountryName.Should().Be("Österreich");
    }

    [Fact]
    public async Task GetByIdAsync_WithUnknownId_ReturnsNull()
    {
        _addresses.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((Address?)null);

        var result = await _handlers.GetByIdAsync(Guid.NewGuid());

        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateAsync_WithUnknownCityAndCountry_CreatesBoth()
    {
        var request = new CreateAddressRequest(
            "Musterfirma GmbH", "Neue Straße", "5", null, "Graz", "8010", "Österreich");

        _countries.FindByNameAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns((Country?)null);
        _cities.FindAsync(
                Arg.Any<string>(), Arg.Any<string>(),
                Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns((City?)null);

        var result = await _handlers.CreateAsync(request);

        result.Should().NotBeNull();
        result.Name.Should().Be("Musterfirma GmbH");
        result.CityName.Should().Be("Graz");

        await _countries.Received(1).AddAsync(
            Arg.Is<Country>(c => c.Name == "Österreich"), Arg.Any<CancellationToken>());
        await _cities.Received(1).AddAsync(
            Arg.Is<City>(c => c.Name == "Graz" && c.ZipCode == "8010"),
            Arg.Any<CancellationToken>());
        await _addresses.Received(1).AddAsync(
            Arg.Any<Address>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateAsync_WithExistingCity_ReusesCityAndCountry()
    {
        var request = new CreateAddressRequest(
            "Anna Schmidt", "Ringstraße", "1", null, "Wien", "1010", "Österreich");

        var country = new Country("Österreich", "AUT");
        _countries.FindByNameAsync("Österreich", Arg.Any<CancellationToken>())
            .Returns(country);
        _cities.FindAsync("Wien", "1010", Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns(new City("Wien", "1010", 1, country));

        var result = await _handlers.CreateAsync(request);

        result.Name.Should().Be("Anna Schmidt");
        await _countries.DidNotReceive().AddAsync(Arg.Any<Country>(), Arg.Any<CancellationToken>());
        await _cities.DidNotReceive().AddAsync(Arg.Any<City>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateAsync_DoesNotCallGetByIdAfterInsert()
    {
        var request = new CreateAddressRequest(
            "Test", "Straße", "1", null, "Berlin", "10115", "Deutschland");

        var country = new Country("Deutschland", "DEU");
        _countries.FindByNameAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(country);
        _cities.FindAsync(Arg.Any<string>(), Arg.Any<string>(),
                Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns(new City("Berlin", "10115", 1, country));

        await _handlers.CreateAsync(request);

        await _addresses.DidNotReceive().GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateAsync_WithUnchangedCity_DoesNotCallResolve()
    {
        var address = CreateTestAddress();
        _addresses.GetByIdAsync(address.Id, Arg.Any<CancellationToken>()).Returns(address);

        var request = new UpdateAddressRequest(
            "Neuer Name", "Neue Straße", "99", null, "Wien", "1010", "Österreich");

        var result = await _handlers.UpdateAsync(address.Id, request);

        result.Should().NotBeNull();
        result!.Name.Should().Be("Neuer Name");

        await _countries.DidNotReceive()
            .FindByNameAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
        await _cities.DidNotReceive()
            .FindAsync(Arg.Any<string>(), Arg.Any<string>(),
                Arg.Any<int>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateAsync_WithChangedCity_CallsResolve()
    {
        var address = CreateTestAddress();
        _addresses.GetByIdAsync(address.Id, Arg.Any<CancellationToken>()).Returns(address);

        var country = new Country("Deutschland", "DEU");
        _countries.FindByNameAsync("Deutschland", Arg.Any<CancellationToken>()).Returns(country);
        _cities.FindAsync("Berlin", "10115", Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns(new City("Berlin", "10115", 1, country));

        var request = new UpdateAddressRequest(
            "Max Mustermann", "Hauptstraße", "12", null, "Berlin", "10115", "Deutschland");

        var result = await _handlers.UpdateAsync(address.Id, request);

        result.Should().NotBeNull();
        result!.CityName.Should().Be("Berlin");

        await _countries.Received(1)
            .FindByNameAsync("Deutschland", Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateAsync_DoesNotCallGetByIdAfterUpdate()
    {
        var address = CreateTestAddress();
        _addresses.GetByIdAsync(address.Id, Arg.Any<CancellationToken>()).Returns(address);

        var request = new UpdateAddressRequest(
            "Max Mustermann", "Hauptstraße", "12", null, "Wien", "1010", "Österreich");

        await _handlers.UpdateAsync(address.Id, request);

        await _addresses.Received(1)
            .GetByIdAsync(address.Id, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateAsync_WithUnknownId_ReturnsNullAndDoesNotPersist()
    {
        _addresses.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((Address?)null);

        var request = new UpdateAddressRequest(
            "X", "X", "1", null, "Wien", "1010", "Österreich");

        var result = await _handlers.UpdateAsync(Guid.NewGuid(), request);

        result.Should().BeNull();
        await _addresses.DidNotReceive()
            .UpdateAsync(Arg.Any<Address>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteAsync_WithUnknownId_ReturnsFalse()
    {
        _addresses.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((Address?)null);

        var result = await _handlers.DeleteAsync(Guid.NewGuid());

        result.Should().BeFalse();
        await _addresses.DidNotReceive()
            .DeleteAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteAsync_WithExistingId_ReturnsTrue()
    {
        var address = CreateTestAddress();
        _addresses.GetByIdAsync(address.Id, Arg.Any<CancellationToken>()).Returns(address);

        var result = await _handlers.DeleteAsync(address.Id);

        result.Should().BeTrue();
        await _addresses.Received(1)
            .DeleteAsync(address.Id, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task SearchAsync_PassesNameFilterToRepository()
    {
        var address = CreateTestAddress(name: "Erika Muster");
        _addresses.SearchAsync("Erika", null, null, null, 1, 20, "street", "asc", Arg.Any<CancellationToken>())
            .Returns(((IReadOnlyList<Address>)[address], 1));

        var result = await _handlers.SearchAsync(new AddressSearchRequest(Name: "Erika"));

        result.Items.Should().ContainSingle().Which.Name.Should().Be("Erika Muster");
        result.TotalCount.Should().Be(1);
    }
}
