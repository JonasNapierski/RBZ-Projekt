namespace RBZ.Projekt.Models.REST;

public class RatingPatchRequest
{
    public int RatingId { get; set; }
    public int? RatingInstitutionId { get; set; }
    public RatingInstitution RatingInstitution {get; set;}
    public int? MovieId { get; set; }
    public Movie Movie { get; set; }
    public double Rating { get; set; }
}
