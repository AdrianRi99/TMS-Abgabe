using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using TMS.Addresses.Application.Handlers;

namespace TMS.Addresses.Application;

public static class AddressesApplicationExtensions
{
    public static IServiceCollection AddAddressesApplication(this IServiceCollection services)
    {
        services.AddScoped<AddressHandlers>();
        services.AddScoped<CityHandlers>();
        services.AddScoped<CountryHandlers>();

        services.AddValidatorsFromAssemblyContaining<AddressHandlers>();

        return services;
    }
}