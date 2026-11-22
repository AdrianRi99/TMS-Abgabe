namespace TMS.WinForms.Models;

public record CityDto(
    int Id,
    string Name,
    string ZipCode,
    int CountryId,
    string CountryName
);