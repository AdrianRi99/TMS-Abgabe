using TMS.WinForms.Models;

namespace TMS.WinForms.Services;

public interface ITmsApiClient
{
    Task<bool> InitializeAsync();
    Task<PagedResult<AddressDto>> SearchAddressesAsync(
        string? street, string? city, string? country, int page, int pageSize,
        string sortBy, string sortDirection);
    Task<List<CityDto>> GetCitiesAsync();
    Task CreateAddressAsync(CreateAddressRequest request);
    Task UpdateAddressAsync(Guid id, CreateAddressRequest request);
    Task DeleteAddressAsync(Guid id);
}