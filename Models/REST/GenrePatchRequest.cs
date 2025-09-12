namespace RBZ.Projekt.Models.REST;

public class GenrePatchRequest
{
    public int? GenreId { get; set; }
    public string Name  { get; set; } = string.Empty;
}
