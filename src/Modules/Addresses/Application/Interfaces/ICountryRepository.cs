using TMS.Addresses.Domain.Entities;

namespace TMS.Addresses.Application.Interfaces;

public interface ICountryRepository
{
    Task<IReadOnlyList<Country>> GetAllAsync(CancellationToken ct = default);
    Task<Country?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<Country?> FindByNameAsync(string name, CancellationToken ct = default);
    Task AddAsync(Country country, CancellationToken ct = default);
}