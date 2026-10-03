using TMS.Addresses.Domain.Entities;

namespace TMS.Addresses.Application.Interfaces;

public interface IAddressRepository
{
    Task<Address?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<(IReadOnlyList<Address> Items, int TotalCount)> SearchAsync(
        string? street,
        string? cityName,
        string? countryName,
        int page,
        int pageSize,
        CancellationToken ct = default);
    Task AddAsync(Address address, CancellationToken ct = default);
    Task UpdateAsync(Address address, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);


}