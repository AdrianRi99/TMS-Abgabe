using Microsoft.EntityFrameworkCore;
using TMS.Orders.Domain.Entities;

namespace TMS.Orders.Infrastructure.Persistence;

public class OrdersDbContext : DbContext
{
    public const string Schema = "orders";

    public OrdersDbContext(DbContextOptions<OrdersDbContext> options)
        : base(options)
    {
    }

    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(OrdersDbContext).Assembly);
    }
}
