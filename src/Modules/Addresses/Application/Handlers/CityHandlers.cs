using TMS.Addresses.Application.DTOs.CityDTO;
using TMS.Addresses.Application.DTOs.Mappings;
using TMS.Addresses.Application.Interfaces;

namespace TMS.Addresses.Application.Handlers;

public class CityHandlers
{
    private readonly ICityRepository _cities;

    public CityHandlers(ICityRepository cities)
    {
        _cities = cities;
    }

    public async Task<IReadOnlyList<CityDto>> GetAllAsync(CancellationToken ct = default)
    {
        var cities = await _cities.GetAllAsync(ct);
        return cities.Select(c => c.ToDto()).ToList();
    }
}