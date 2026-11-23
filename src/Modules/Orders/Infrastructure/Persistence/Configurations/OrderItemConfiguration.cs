using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TMS.Orders.Domain.Entities;

namespace TMS.Orders.Infrastructure.Persistence.Configurations;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("order_items");

        builder.HasKey(i => i.Id);
        builder.Property(i => i.Id).ValueGeneratedNever();

        builder.Property(i => i.Description)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(i => i.Price).HasPrecision(18, 2);
        builder.Property(i => i.Quantity).IsRequired();
        builder.Property(i => i.Position).IsRequired();

        builder.HasIndex(i => i.OrderId);
    }
}
