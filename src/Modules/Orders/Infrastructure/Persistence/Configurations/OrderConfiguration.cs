using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TMS.Orders.Domain.Entities;

namespace TMS.Orders.Infrastructure.Persistence.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("orders");

        builder.HasKey(o => o.Id);
        builder.Property(o => o.Id).ValueGeneratedNever();

        // Reference to the Addresses module: plain column, deliberately NO foreign key.
        builder.Property(o => o.AddressId).IsRequired();

        builder.Property(o => o.CustomerName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(o => o.OrderDate).IsRequired();

        builder.Property(o => o.Status)
            .HasMaxLength(20)
            .IsRequired();

        builder.HasMany(o => o.Items)
            .WithOne()
            .HasForeignKey(i => i.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(o => o.Items).UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasIndex(o => o.AddressId);
        builder.HasIndex(o => o.OrderDate);
    }
}
