namespace TMS.Addresses.Application.DTOs.AddressDTO;

public record AddressSearchRequest(
    string? Street = null,
    string? CityName = null,
    string? CountryName = null,
    int Page = 1,
    int PageSize = 20
);