namespace RBZ.Projekt.Models.REST;

public class CurrencyPatchRequest
{
    public int? CurrencyId { get; set; }
    public string Symbol { get; set; } = string.Empty;
}
