using TMS.Addresses.Domain.Entities;

namespace TMS.Addresses.Application.Interfaces;

public interface ICityRepository
{
    Task<IReadOnlyList<City>> GetAllAsync(CancellationToken ct = default);
    Task<City?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<City?> FindAsync(string name, string zipCode, CancellationToken ct = default);
    Task AddAsync(City city, CancellationToken ct = default);
}