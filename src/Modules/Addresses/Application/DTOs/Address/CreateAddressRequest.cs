namespace TMS.Addresses.Application.DTOs.AddressDTO;


public record CreateAddressRequest(
    string Street,
    string HouseNumber,
    string? Supplement,
    string CityName,
    string ZipCode,
    string CountryName
);