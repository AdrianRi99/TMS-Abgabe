using Microsoft.Extensions.Logging;
using TMS.Addresses.Application.DTOs.AddressDTO;
using TMS.Addresses.Application.Common;
using TMS.Addresses.Application.Interfaces;
using TMS.Addresses.Application.DTOs.Mappings;
using TMS.Addresses.Domain.Entities;

namespace TMS.Addresses.Application.Handlers;

public class AddressHandlers
{
    private readonly IAddressRepository _addresses;
    private readonly ICityRepository _cities;
    private readonly ICountryRepository _countries;
    private readonly ILogger<AddressHandlers> _logger;

    public AddressHandlers(
        IAddressRepository addresses,
        ICityRepository cities,
        ICountryRepository countries,
        ILogger<AddressHandlers> logger)
    {
        _addresses = addresses;
        _cities = cities;
        _countries = countries;
        _logger = logger;
    }

    public async Task<AddressDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        _logger.LogDebug("Fetching address {AddressId}", id);
        var address = await _addresses.GetByIdAsync(id, ct);
        if (address is null)
            _logger.LogWarning("Address {AddressId} not found", id);
        return address?.ToDto();
    }

    public async Task<PagedResult<AddressDto>> SearchAsync(
        AddressSearchRequest request, CancellationToken ct = default)
    {
        _logger.LogDebug(
            "Searching addresses: street={Street} city={City} country={Country} page={Page} sortBy={SortBy} sortDirection={SortDirection}",
            request.Street, request.CityName, request.CountryName,
            request.Page, request.SortBy, request.SortDirection);

        var (items, totalCount) = await _addresses.SearchAsync(
            request.Street, request.CityName, request.CountryName,
            request.Page, request.PageSize,
            request.SortBy, request.SortDirection, ct);

        _logger.LogDebug("Search returned {Count} of {Total} addresses", items.Count, totalCount);

        return new PagedResult<AddressDto>(
            items.Select(a => a.ToDto()).ToList(),
            totalCount, request.Page, request.PageSize);
    }

    public async Task<AddressDto> CreateAsync(
        CreateAddressRequest request, CancellationToken ct = default)
    {
        _logger.LogInformation(
            "Creating address: {Name}, {Street} {HouseNumber}, {CityName}, {CountryName}",
            request.Name, request.Street, request.HouseNumber, request.CityName, request.CountryName);

        var (city, country) = await ResolveOrCreateCityAsync(
            request.CityName, request.ZipCode, request.CountryName, ct);

        var address = new Address(
            request.Name, request.Street, request.HouseNumber,
            city.Id, request.Supplement);

        await _addresses.AddAsync(address, ct);

        _logger.LogInformation("Address created with id {AddressId}", address.Id);

        return new AddressDto(
            address.Id,
            address.Name,
            address.Street,
            address.HouseNumber,
            address.Supplement,
            city.Id,
            city.Name,
            city.ZipCode,
            country.Id,
            country.Name);
    }

    public async Task<AddressDto?> UpdateAsync(
        Guid id, UpdateAddressRequest request, CancellationToken ct = default)
    {
        var address = await _addresses.GetByIdAsync(id, ct);
        if (address is null)
        {
            _logger.LogWarning("Update failed: address {AddressId} not found", id);
            return null;
        }

        _logger.LogInformation("Updating address {AddressId}", id);

        City city;
        Country country;

        var cityUnchanged =
            address.City.Name.Equals(request.CityName, StringComparison.OrdinalIgnoreCase) &&
            address.City.ZipCode == request.ZipCode &&
            address.City.Country.Name.Equals(request.CountryName, StringComparison.OrdinalIgnoreCase);

        if (cityUnchanged)
        {
            city = address.City;
            country = address.City.Country;
        }
        else
        {
            (city, country) = await ResolveOrCreateCityAsync(
                request.CityName, request.ZipCode, request.CountryName, ct);
        }

        address.Update(request.Name, request.Street, request.HouseNumber,
            city.Id, request.Supplement);
        await _addresses.UpdateAsync(address, ct);

        return new AddressDto(
            address.Id,
            address.Name,
            address.Street,
            address.HouseNumber,
            address.Supplement,
            city.Id,
            city.Name,
            city.ZipCode,
            country.Id,
            country.Name);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var address = await _addresses.GetByIdAsync(id, ct);
        if (address is null)
        {
            _logger.LogWarning("Delete failed: address {AddressId} not found", id);
            return false;
        }

        _logger.LogInformation("Deleting address {AddressId}", id);
        await _addresses.DeleteAsync(id, ct);
        return true;
    }

    private async Task<(City City, Country Country)> ResolveOrCreateCityAsync(
        string cityName, string zipCode, string countryName, CancellationToken ct)
    {
        var country = await _countries.FindByNameAsync(countryName, ct);
        if (country is null)
        {
            _logger.LogInformation("Creating new country: {CountryName}", countryName);
            country = new Country(countryName);
            await _countries.AddAsync(country, ct);
        }

        var city = await _cities.FindAsync(cityName, zipCode, country.Id, ct);
        if (city is null)
        {
            _logger.LogInformation(
                "Creating new city: {CityName} {ZipCode} in {CountryName}",
                cityName, zipCode, countryName);
            city = new City(cityName, zipCode, country.Id);
            await _cities.AddAsync(city, ct);
        }

        return (city, country);
    }
}