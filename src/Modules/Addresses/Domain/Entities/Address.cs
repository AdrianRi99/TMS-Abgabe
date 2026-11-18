namespace TMS.Addresses.Domain.Entities;

public class Address
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Street { get; private set; } = string.Empty;
    public string HouseNumber { get; private set; } = string.Empty;
    public string? Supplement { get; private set; }

    public int CityId { get; private set; }
    public City City { get; private set; } = null!;

    private Address() { }

    public Address(string name, string street, string houseNumber,
        int cityId, string? supplement = null, City? city = null)
    {
        Id = Guid.NewGuid();
        Name = name;
        Street = street;
        HouseNumber = houseNumber;
        CityId = cityId;
        Supplement = supplement;
        if (city is not null) City = city;
    }

    public void Update(string name, string street, string houseNumber,
        int cityId, string? supplement = null)
    {
        Name = name;
        Street = street;
        HouseNumber = houseNumber;
        CityId = cityId;
        Supplement = supplement;
    }
}