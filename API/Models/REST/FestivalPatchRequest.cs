namespace RBZ.Projekt.Models.REST;

public class FestivalPatchRequest
{
    public int? FestivalId { get; set; }
    public string Name  { get; set; } = string.Empty;
    public int Year { get; set; }
    public string Location { get; set; } = string.Empty;
    public int MovieId { get; set; }
    public Movie Movie { get; set; }
}
