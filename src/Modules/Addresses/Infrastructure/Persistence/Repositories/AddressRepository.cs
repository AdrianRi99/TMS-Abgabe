using Microsoft.EntityFrameworkCore;
using TMS.Addresses.Application.Interfaces;
using TMS.Addresses.Domain.Entities;

namespace TMS.Addresses.Infrastructure.Persistence.Repositories;

public class AddressRepository : IAddressRepository
{
    private readonly AddressesDbContext _context;

    public AddressRepository(AddressesDbContext context)
    {
        _context = context;
    }

    public async Task<Address?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.Addresses
            .AsNoTracking()
            .Include(a => a.City)
            .ThenInclude(c => c.Country)
            .FirstOrDefaultAsync(a => a.Id == id, ct);
    }

    public async Task<(IReadOnlyList<Address> Items, int TotalCount)> SearchAsync(
    string? street,
    string? cityName,
    string? countryName,
    int page,
    int pageSize,
    CancellationToken ct = default)
    {
        var query = _context.Addresses
            .AsNoTracking()
            .Include(a => a.City)
            .ThenInclude(c => c.Country)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(street))
            query = query.Where(a => EF.Functions.ILike(a.Street, $"%{street}%"));

        if (!string.IsNullOrWhiteSpace(cityName))
            query = query.Where(a => EF.Functions.ILike(a.City.Name, $"%{cityName}%"));

        if (!string.IsNullOrWhiteSpace(countryName))
            query = query.Where(a => EF.Functions.ILike(a.City.Country.Name, $"%{countryName}%"));

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .OrderBy(a => a.Street)
            .ThenBy(a => a.HouseNumber)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, totalCount);
    }

    public async Task AddAsync(Address address, CancellationToken ct = default)
    {
        await _context.Addresses.AddAsync(address, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Address address, CancellationToken ct = default)
    {
        var tracked = await _context.Addresses.FindAsync([address.Id], ct)
            ?? throw new InvalidOperationException($"Address {address.Id} not found.");

        _context.Entry(tracked).CurrentValues.SetValues(address);
        await _context.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var address = await _context.Addresses.FindAsync([id], ct);
        if (address is null) return;

        _context.Addresses.Remove(address);
        await _context.SaveChangesAsync(ct);
    }

}