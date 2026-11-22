using TMS.WinForms.Models;
using TMS.WinForms.Services;

namespace TMS.WinForms;

public sealed class AddressesViewModel
{
    private readonly ITmsApiClient _api;

    public AddressesViewModel(ITmsApiClient api)
    {
        _api = api;
    }

    // --- State ---
    public List<AddressDto> Addresses { get; private set; } = [];
    public List<CityDto> Cities { get; private set; } = [];
    public int CurrentPage { get; private set; } = 1;
    public int TotalPages { get; private set; } = 1;
    public const int PageSize = 20;

    public string FilterStreet { get; set; } = "";
    public string FilterCity { get; set; } = "";
    public string FilterCountry { get; set; } = "";

    public string SortBy { get; private set; } = "street";
    public string SortDirection { get; private set; } = "asc";

    private static readonly Dictionary<int, string> ColumnSortKeys = new()
    {
        [0] = "name",
        [1] = "street",
        [2] = "housenumber",
        [4] = "zipcode",
        [5] = "city",
        [6] = "country"
    };

    // --- Lifecycle ---
    public async Task<bool> InitializeAsync()
    {
        var connected = await _api.InitializeAsync();
        if (!connected) return false;

        Cities = await _api.GetCitiesAsync();
        await LoadAddressesAsync();
        return true;
    }

    public async Task LoadAddressesAsync()
    {
        var result = await _api.SearchAddressesAsync(
            FilterStreet, FilterCity, FilterCountry,
            CurrentPage, PageSize, SortBy, SortDirection);

        Addresses = result.Items;
        TotalPages = Math.Max(1, result.TotalPages);
    }

    // --- Filter actions ---
    public async Task SearchAsync()
    {
        CurrentPage = 1;
        await LoadAddressesAsync();
    }

    public async Task ResetFilterAsync()
    {
        FilterStreet = "";
        FilterCity = "";
        FilterCountry = "";
        await SearchAsync();
    }

    // --- Sort actions ---
    public bool TryGetSortField(int columnIndex, out string field) =>
        ColumnSortKeys.TryGetValue(columnIndex, out field!);

    public async Task SortAsync(string field)
    {
        if (SortBy == field)
        {
            SortDirection = SortDirection == "asc" ? "desc" : "asc";
        }
        else
        {
            SortBy = field;
            SortDirection = "asc";
        }

        CurrentPage = 1;
        await LoadAddressesAsync();
    }

    // --- Paging actions ---
    public bool CanGoNext => CurrentPage < TotalPages;
    public bool CanGoPrevious => CurrentPage > 1;

    public async Task NextPageAsync()
    {
        if (!CanGoNext) return;
        CurrentPage++;
        await LoadAddressesAsync();
    }

    public async Task PreviousPageAsync()
    {
        if (!CanGoPrevious) return;
        CurrentPage--;
        await LoadAddressesAsync();
    }

    // --- CRUD ---
    public async Task CreateAddressAsync(CreateAddressRequest request)
    {
        await _api.CreateAddressAsync(request);
        await LoadAddressesAsync();
    }

    public async Task UpdateAddressAsync(Guid id, CreateAddressRequest request)
    {
        await _api.UpdateAddressAsync(id, request);
        await LoadAddressesAsync();
    }

    public async Task DeleteAddressAsync(Guid id)
    {
        await _api.DeleteAddressAsync(id);
        await LoadAddressesAsync();
    }
}