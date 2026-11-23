namespace TMS.Orders.Domain;

public static class OrderStatus
{
    public const string Open = "Offen";
    public const string Completed = "Abgeschlossen";
    public const string Cancelled = "Storniert";

    public static readonly IReadOnlyList<string> All = [Open, Completed, Cancelled];

    public static bool IsValid(string? status) => status is not null && All.Contains(status);
}
