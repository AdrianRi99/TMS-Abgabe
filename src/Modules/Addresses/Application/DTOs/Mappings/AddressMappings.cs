using TMS.Addresses.Domain.Entities;
using TMS.Addresses.Application.DTOs.AddressDTO;
using TMS.Addresses.Application.DTOs.CityDTO;
using TMS.Addresses.Application.DTOs.CountryDTO;


namespace TMS.Addresses.Application.DTOs.Mappings;

public static class AddressMappings
{
    public static AddressDto ToDto(this Address a) => new(
        a.Id,
        a.Street,
        a.HouseNumber,
        a.Supplement,
        a.CityId,
        a.City.Name,
        a.City.ZipCode,
        a.City.CountryId,
        a.City.Country.Name
    );

    public static CityDto ToDto(this City c) => new(
        c.Id,
        c.Name,
        c.ZipCode,
        c.CountryId,
        c.Country.Name
    );

    public static CountryDto ToDto(this Country c) => new(
        c.Id,
        c.Name,
        c.IsoCode
    );
}