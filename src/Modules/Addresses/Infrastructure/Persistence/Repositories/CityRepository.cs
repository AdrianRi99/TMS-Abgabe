using Microsoft.EntityFrameworkCore;
using TMS.Addresses.Application.Interfaces;
using TMS.Addresses.Domain.Entities;

namespace TMS.Addresses.Infrastructure.Persistence.Repositories;

public class CityRepository : ICityRepository
{
    private readonly AddressesDbContext _context;

    public CityRepository(AddressesDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<City>> GetAllAsync(CancellationToken ct = default)
    {
        return await _context.Cities
            .AsNoTracking()
            .Include(c => c.Country)
            .OrderBy(c => c.Name)
            .ToListAsync(ct);
    }

    public async Task<City?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _context.Cities
            .AsNoTracking()
            .Include(c => c.Country)
            .FirstOrDefaultAsync(c => c.Id == id, ct);
    }

    public async Task<City?> FindAsync(
    string name, string zipCode, int countryId, CancellationToken ct = default)
    {
        var lowered = name.ToLower();
        return await _context.Cities.FirstOrDefaultAsync(c =>
            c.CountryId == countryId &&
            c.ZipCode == zipCode &&
            c.Name.ToLower() == lowered, ct);
    }

    public async Task AddAsync(City city, CancellationToken ct = default)
    {
        await _context.Cities.AddAsync(city, ct);
        await _context.SaveChangesAsync(ct);
    }
}