namespace TMS.Addresses.Application.DTOs.AddressDTO;

public record UpdateAddressRequest(
    string Name,
    string Street,
    string HouseNumber,
    string? Supplement,
    string CityName,
    string ZipCode,
    string CountryName
);