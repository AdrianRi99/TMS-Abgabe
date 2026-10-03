using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TMS.Addresses.Domain.Entities;
using TMS.Addresses.Infrastructure.Persistence;
using TMS.Identity.Infrastructure.Persistence;

namespace TMS.Api;

public static class SeedData
{
    private const string DemoEmail = "demo@tms.dev";
    private const string DemoPassword = "Demo1234";

    public static async Task SeedAsync(
        AddressesDbContext addressesDb,
        UserManager<ApplicationUser> userManager)
    {
        await SeedDemoUserAsync(userManager);
        await SeedAddressesAsync(addressesDb);
    }

    private static async Task SeedDemoUserAsync(UserManager<ApplicationUser> userManager)
    {
        if (await userManager.FindByEmailAsync(DemoEmail) is not null) return;

        var user = new ApplicationUser { UserName = DemoEmail, Email = DemoEmail };
        var result = await userManager.CreateAsync(user, DemoPassword);

        if (!result.Succeeded)
            throw new Exception($"Demo-User konnte nicht angelegt werden: " +
                string.Join(", ", result.Errors.Select(e => e.Description)));
    }

    private static async Task SeedAddressesAsync(AddressesDbContext context)
    {
        if (await context.Countries.AnyAsync()) return;

        var austria = new Country("Österreich", "AUT");
        var germany = new Country("Deutschland", "DEU");
        var switzerland = new Country("Schweiz", "CHE");
        var netherlands = new Country("Niederlande", "NLD");
        var italy = new Country("Italien", "ITA");

        await context.Countries.AddRangeAsync(austria, germany, switzerland, netherlands, italy);
        await context.SaveChangesAsync();

        var wien = new City("Wien", "1010", austria.Id);
        var graz = new City("Graz", "8010", austria.Id);
        var salzburg = new City("Salzburg", "5020", austria.Id);
        var berlin = new City("Berlin", "10115", germany.Id);
        var munich = new City("München", "80331", germany.Id);
        var cologne = new City("Köln", "50667", germany.Id);
        var stuttgart = new City("Stuttgart", "70173", germany.Id);
        var hamburg = new City("Hamburg", "20095", germany.Id);
        var frankfurt = new City("Frankfurt am Main", "60313", germany.Id);
        var zurich = new City("Zürich", "8001", switzerland.Id);
        var basel = new City("Basel", "4051", switzerland.Id);
        var amsterdam = new City("Amsterdam", "1012", netherlands.Id);
        var rome = new City("Rom", "00184", italy.Id);
        var milan = new City("Mailand", "20121", italy.Id);

        await context.Cities.AddRangeAsync(
            wien, graz, salzburg,
            berlin, munich, cologne, stuttgart, hamburg, frankfurt,
            zurich, basel,
            amsterdam,
            rome, milan);
        await context.SaveChangesAsync();

        var addresses = new List<Address>
        {
            // Deutschland
            new("Musterstraße", "12", berlin.Id),
            new("Hauptstraße", "45a", munich.Id, "2. OG"),
            new("Gartenweg", "5", cologne.Id),
            new("Industriestraße", "18", stuttgart.Id, "Halle 3"),
            new("Schillerstraße", "3", berlin.Id),
            new("Leopoldstraße", "99", munich.Id),
            new("Kurfürstendamm", "200", berlin.Id),
            new("Reeperbahn", "14", hamburg.Id, "Hinterhaus"),
            new("Speicherstadt", "7", hamburg.Id),
            new("Zeil", "106", frankfurt.Id),
            new("Sachsenhäuser Ufer", "33", frankfurt.Id, "EG"),
            new("Domstraße", "1", cologne.Id),

            // Österreich
            new("Ringstraße", "23", wien.Id, "Hinterhaus"),
            new("Praterstraße", "44", wien.Id),
            new("Mozartgasse", "11", graz.Id),
            new("Herrengasse", "5", graz.Id, "Büro 3"),
            new("Getreidegasse", "9", salzburg.Id),
            new("Linzer Gasse", "28", salzburg.Id),

            // Schweiz
            new("Bahnhofstraße", "7", zurich.Id),
            new("Seestraße", "88", zurich.Id),
            new("Freie Straße", "12", basel.Id),
            new("Marktplatz", "3", basel.Id, "1. OG"),

            // Niederlande
            new("Herengracht", "182", amsterdam.Id),
            new("Damrak", "55", amsterdam.Id, "Zimmer 4"),
            new("Keizersgracht", "240", amsterdam.Id),

            // Italien
            new("Via del Corso", "18", rome.Id),
            new("Via Veneto", "54", rome.Id, "Suite 12"),
            new("Piazza di Spagna", "3", rome.Id),
            new("Via Montenapoleone", "8", milan.Id),
            new("Corso Buenos Aires", "77", milan.Id),
        };

        await context.Addresses.AddRangeAsync(addresses);
        await context.SaveChangesAsync();
    }
}