using TMS.Addresses.Application.Handlers;
using TMS.Orders.Application.Interfaces;

namespace TMS.Api.Adapters;

/// <summary>
/// Composition-root adapter: lets the Orders module resolve a customer name from the
/// Addresses module without Orders referencing Addresses.
/// </summary>
public class AddressCustomerLookup : ICustomerLookup
{
    private readonly AddressHandlers _addresses;

    public AddressCustomerLookup(AddressHandlers addresses)
    {
        _addresses = addresses;
    }

    public async Task<string?> GetCustomerNameAsync(Guid addressId, CancellationToken ct = default)
    {
        var address = await _addresses.GetByIdAsync(addressId, ct);
        return address?.Name;
    }
}
