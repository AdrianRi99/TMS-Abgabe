using Microsoft.EntityFrameworkCore;
using TMS.Addresses.Application.Interfaces;
using TMS.Addresses.Domain.Entities;

namespace TMS.Addresses.Infrastructure.Persistence.Repositories;

public class CountryRepository : ICountryRepository
{
    private readonly AddressesDbContext _context;

    public CountryRepository(AddressesDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Country>> GetAllAsync(CancellationToken ct = default)
    {
        return await _context.Countries
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .ToListAsync(ct);
    }

    public async Task<Country?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _context.Countries
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id, ct);
    }

    public async Task<Country?> FindByNameAsync(string name, CancellationToken ct = default)
    {
        return await _context.Countries
            .FirstOrDefaultAsync(c => c.Name.ToLower() == name.ToLower(), ct);
    }

    public async Task AddAsync(Country country, CancellationToken ct = default)
    {
        await _context.Countries.AddAsync(country, ct);
        await _context.SaveChangesAsync(ct);
    }
}