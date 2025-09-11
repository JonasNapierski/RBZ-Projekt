namespace RBZ.Projekt.Models.REST;

public class CountryPatchRequest
{
    public int Id {get; set;}
    public string Name {get; set;} = string.Empty;
    public string CurrencySymbol {get; set;} = string.Empty;

}
