namespace TMS.Addresses.Domain.Entities;

public class Country
{
    public int Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string? IsoCode { get; private set; }

    public ICollection<City> Cities { get; private set; } = new List<City>();

    private Country() { }

    public Country(string name, string? isoCode = null)
    {
        Name = name;
        IsoCode = isoCode;
    }
}