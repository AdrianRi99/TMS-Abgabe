namespace TMS.Addresses.Application.DTOs.AddressDTO;

public record AddressDto(
    Guid Id,
    string Street,
    string HouseNumber,
    string? Supplement,
    int CityId,
    string CityName,
    string ZipCode,
    int CountryId,
    string CountryName
);