using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TMS.Orders.Application.Interfaces;
using TMS.Orders.Infrastructure.Persistence;
using TMS.Orders.Infrastructure.Persistence.Repositories;

namespace TMS.Orders.Infrastructure;

public static class OrdersInfrastructureExtensions
{
    public static IServiceCollection AddOrdersInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<OrdersDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                npgsql => npgsql.MigrationsHistoryTable("__EFMigrationsHistory", OrdersDbContext.Schema)));

        services.AddScoped<IOrderRepository, OrderRepository>();

        return services;
    }
}
