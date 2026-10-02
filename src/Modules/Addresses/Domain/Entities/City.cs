namespace TMS.Addresses.Domain.Entities;

public class City
{
    public int Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string ZipCode { get; private set; } = string.Empty;

    public int CountryId { get; private set; }
    public Country Country { get; private set; } = null!;

    public ICollection<Address> Addresses { get; private set; } = new List<Address>();

    private City() { }

    public City(string name, string zipCode, int countryId, Country? country = null)
    {
        Name = name;
        ZipCode = zipCode;
        CountryId = countryId;
        if (country is not null) Country = country;
    }
}