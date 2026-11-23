using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using TMS.Orders.Application.Handlers;

namespace TMS.Orders.Application;

public static class OrdersApplicationExtensions
{
    public static IServiceCollection AddOrdersApplication(this IServiceCollection services)
    {
        services.AddScoped<OrderHandlers>();
        services.AddValidatorsFromAssemblyContaining<OrderHandlers>();
        return services;
    }
}
