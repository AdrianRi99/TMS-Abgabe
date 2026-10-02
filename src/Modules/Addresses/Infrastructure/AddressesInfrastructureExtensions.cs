using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TMS.Addresses.Application.Interfaces;
using TMS.Addresses.Infrastructure.Persistence;
using TMS.Addresses.Infrastructure.Persistence.Repositories;

namespace TMS.Addresses.Infrastructure;

public static class AddressesInfrastructureExtensions
{
    public static IServiceCollection AddAddressesInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<AddressesDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IAddressRepository, AddressRepository>();
        services.AddScoped<ICityRepository, CityRepository>();
        services.AddScoped<ICountryRepository, CountryRepository>();

        return services;
    }
}