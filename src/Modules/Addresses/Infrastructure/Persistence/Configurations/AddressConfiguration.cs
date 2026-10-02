using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TMS.Addresses.Domain.Entities;

namespace TMS.Addresses.Infrastructure.Persistence.Configurations;

public class AddressConfiguration : IEntityTypeConfiguration<Address>
{
    public void Configure(EntityTypeBuilder<Address> builder)
    {
        builder.ToTable("addresses");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Street)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(a => a.HouseNumber)
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(a => a.Supplement)
            .HasMaxLength(100);

        builder.HasOne(a => a.City)
            .WithMany(c => c.Addresses)
            .HasForeignKey(a => a.CityId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(a => a.Street);
        builder.HasIndex(a => a.CityId);
    }
}