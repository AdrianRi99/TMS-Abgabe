using System.Net.Http.Headers;
using System.Net.Http.Json;
using TMS.WinForms.Models;

namespace TMS.WinForms.Services;

public sealed class TmsApiClient : ITmsApiClient
{
    private readonly HttpClient _httpClient;
    private readonly AuthSession _auth;

    public TmsApiClient(HttpClient httpClient, AuthSession auth)
    {
        _httpClient = httpClient;
        _auth = auth;
    }

    public async Task<bool> InitializeAsync()
    {
        var success = await _auth.LoginAsDemoAsync(_httpClient);
        if (success)
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _auth.Token);
        }
        return success;
    }

    public async Task<PagedResult<AddressDto>> SearchAddressesAsync(
        string? street, string? city, string? country, int page, int pageSize,
        string sortBy, string sortDirection)
    {
        var query = $"api/addresses?page={page}&pageSize={pageSize}" +
                    $"&sortBy={sortBy}&sortDirection={sortDirection}";

        if (!string.IsNullOrWhiteSpace(street)) query += $"&street={Uri.EscapeDataString(street)}";
        if (!string.IsNullOrWhiteSpace(city)) query += $"&cityName={Uri.EscapeDataString(city)}";
        if (!string.IsNullOrWhiteSpace(country)) query += $"&countryName={Uri.EscapeDataString(country)}";

        var result = await _httpClient.GetFromJsonAsync<PagedResult<AddressDto>>(query);
        return result ?? new PagedResult<AddressDto>([], 0, page, pageSize, 0);
    }

    public async Task<List<CityDto>> GetCitiesAsync()
    {
        var result = await _httpClient.GetFromJsonAsync<List<CityDto>>("api/cities");
        return result ?? [];
    }

    public async Task CreateAddressAsync(CreateAddressRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("api/addresses", request);
        response.EnsureSuccessStatusCode();
    }

    public async Task UpdateAddressAsync(Guid id, CreateAddressRequest request)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/addresses/{id}", request);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteAddressAsync(Guid id)
    {
        var response = await _httpClient.DeleteAsync($"api/addresses/{id}");
        response.EnsureSuccessStatusCode();
    }
}