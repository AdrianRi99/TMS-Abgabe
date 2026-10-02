using TMS.Addresses.Application.DTOs.CountryDTO;
using TMS.Addresses.Application.DTOs.Mappings;
using TMS.Addresses.Application.Interfaces;

namespace TMS.Addresses.Application.Handlers;

public class CountryHandlers
{
    private readonly ICountryRepository _countries;

    public CountryHandlers(ICountryRepository countries)
    {
        _countries = countries;
    }

    public async Task<IReadOnlyList<CountryDto>> GetAllAsync(CancellationToken ct = default)
    {
        var countries = await _countries.GetAllAsync(ct);
        return countries.Select(c => c.ToDto()).ToList();
    }
}