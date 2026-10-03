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

    public AddressHandlers(
        IAddressRepository addresses,
        ICityRepository cities,
        ICountryRepository countries)
    {
        _addresses = addresses;
        _cities = cities;
        _countries = countries;
    }

    public async Task<AddressDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var address = await _addresses.GetByIdAsync(id, ct);
        return address?.ToDto();
    }

    public async Task<PagedResult<AddressDto>> SearchAsync(
        AddressSearchRequest request,
        CancellationToken ct = default)
    {
        var (items, totalCount) = await _addresses.SearchAsync(
            request.Street,
            request.CityName,
            request.CountryName,
            request.Page,
            request.PageSize,
            ct);

        return new PagedResult<AddressDto>(
            items.Select(a => a.ToDto()).ToList(),
            totalCount,
            request.Page,
            request.PageSize);
    }

    public async Task<AddressDto> CreateAsync(CreateAddressRequest request, CancellationToken ct = default)
    {
        var cityId = await ResolveOrCreateCityAsync(request.CityName, request.ZipCode, request.CountryName, ct);
        var address = new Address(request.Street, request.HouseNumber, cityId, request.Supplement);
        await _addresses.AddAsync(address, ct);
        var created = await _addresses.GetByIdAsync(address.Id, ct);
        return created!.ToDto();
    }

    public async Task<AddressDto?> UpdateAsync(Guid id, UpdateAddressRequest request, CancellationToken ct = default)
    {
        var address = await _addresses.GetByIdAsync(id, ct);
        if (address is null) return null;

        var cityId = await ResolveOrCreateCityAsync(request.CityName, request.ZipCode, request.CountryName, ct);
        address.Update(request.Street, request.HouseNumber, cityId, request.Supplement);
        await _addresses.UpdateAsync(address, ct);

        var updated = await _addresses.GetByIdAsync(id, ct);
        return updated!.ToDto();
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var address = await _addresses.GetByIdAsync(id, ct);
        if (address is null) return false;
        await _addresses.DeleteAsync(id, ct);
        return true;
    }

    private async Task<int> ResolveOrCreateCityAsync(
    string cityName, string zipCode, string countryName, CancellationToken ct)
    {
        var country = await _countries.FindByNameAsync(countryName, ct);
        if (country is null)
        {
            country = new Country(countryName);
            await _countries.AddAsync(country, ct);
        }

        var city = await _cities.FindAsync(cityName, zipCode, country.Id, ct);
        if (city is not null) return city.Id;

        city = new City(cityName, zipCode, country.Id);
        await _cities.AddAsync(city, ct);
        return city.Id;
    }


}