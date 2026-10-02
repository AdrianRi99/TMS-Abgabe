namespace TMS.Addresses.Application.DTOs.CityDTO;

public record CityDto(
    int Id,
    string Name,
    string ZipCode,
    int CountryId,
    string CountryName
);