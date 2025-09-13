namespace RBZ.Projekt.Models.REST;

public class MovieCreateRequest
{
    public string Title { get; set; }
    public long budget { get; set; }
    public long? RevenueInternational { get; set; }
    public long? RevenueDomestic { get; set; }
    public int Year { get; set; }
    public string Country { get; set; }
}
