using Microsoft.EntityFrameworkCore;
using TMS.Addresses.Domain.Entities;

namespace TMS.Addresses.Infrastructure.Persistence;

public class AddressesDbContext : DbContext
{
    public AddressesDbContext(DbContextOptions<AddressesDbContext> options)
        : base(options)
    {
    }

    public DbSet<Address> Addresses => Set<Address>();
    public DbSet<City> Cities => Set<City>();
    public DbSet<Country> Countries => Set<Country>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AddressesDbContext).Assembly);
    }
}