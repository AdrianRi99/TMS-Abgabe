namespace TMS.WinForms.Models;

public record AddressDto(
    Guid Id,
    string Name,
    string Street,
    string HouseNumber,
    string? Supplement,
    int CityId,
    string CityName,
    string ZipCode,
    int CountryId,
    string CountryName
);

public record CreateAddressRequest(
    string Name,
    string Street,
    string HouseNumber,
    string? Supplement,
    string CityName,
    string ZipCode,
    string CountryName
);