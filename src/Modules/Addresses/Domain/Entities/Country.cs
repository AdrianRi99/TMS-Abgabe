namespace TMS.Addresses.Domain.Entities;

public class Country
{
    public int Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string IsoCode { get; private set; } = string.Empty;

    public ICollection<City> Cities { get; private set; } = new List<City>();

    private Country() { }

    public Country(string name, string isoCode)
    {
        Name = name;
        IsoCode = isoCode;
    }
}