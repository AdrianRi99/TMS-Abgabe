namespace TMS.Orders.Application.Interfaces;

/// <summary>
/// Port to resolve the customer name for an address without coupling Orders to the Addresses module.
/// Implemented by an adapter in the host (composition root).
/// </summary>
public interface ICustomerLookup
{
    /// <returns>The customer name, or null if the address does not exist.</returns>
    Task<string?> GetCustomerNameAsync(Guid addressId, CancellationToken ct = default);
}
