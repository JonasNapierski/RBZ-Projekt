namespace RBZ.Projekt.Models.REST;

public class RatingInstitutionPatchRequest
{
    public int? GenreId { get; set; }
    public string Name  { get; set; } = string.Empty;
}
