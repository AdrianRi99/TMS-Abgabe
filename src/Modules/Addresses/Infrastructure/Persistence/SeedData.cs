using Microsoft.EntityFrameworkCore;
using TMS.Addresses.Domain.Entities;
using TMS.Addresses.Infrastructure.Persistence;

namespace TMS.Addresses.Infrastructure.Persistence;

public static class SeedData
{
    public static async Task SeedAsync(AddressesDbContext context)
    {
        if (await context.Countries.AnyAsync()) return;

        var austria = new Country("Österreich", "AUT");
        var germany = new Country("Deutschland", "DEU");
        var switzerland = new Country("Schweiz", "CHE");

        await context.Countries.AddRangeAsync(austria, germany, switzerland);
        await context.SaveChangesAsync();

        var wien = new City("Wien", "1010", austria.Id);
        var graz = new City("Graz", "8010", austria.Id);
        var berlin = new City("Berlin", "10115", germany.Id);
        var munich = new City("München", "80331", germany.Id);
        var cologne = new City("Köln", "50667", germany.Id);
        var stuttgart = new City("Stuttgart", "70173", germany.Id);
        var zurich = new City("Zürich", "8001", switzerland.Id);

        await context.Cities.AddRangeAsync(wien, graz, berlin, munich, cologne, stuttgart, zurich);
        await context.SaveChangesAsync();

        var addresses = new List<Address>
        {
            new("Musterstraße", "12", berlin.Id),
            new("Hauptstraße", "45a", munich.Id, "2. OG"),
            new("Bahnhofstraße", "7", zurich.Id),
            new("Ringstraße", "23", wien.Id, "Hinterhaus"),
            new("Gartenweg", "5", cologne.Id),
            new("Industriestraße", "18", stuttgart.Id, "Halle 3"),
            new("Schillerstraße", "3", berlin.Id),
            new("Mozartgasse", "11", graz.Id),
            new("Seestraße", "88", zurich.Id),
            new("Praterstraße", "44", wien.Id),
            new("Leopoldstraße", "99", munich.Id),
            new("Kurfürstendamm", "200", berlin.Id),
        };

        await context.Addresses.AddRangeAsync(addresses);
        await context.SaveChangesAsync();
    }
}