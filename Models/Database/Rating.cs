using System.ComponentModel.DataAnnotations;

namespace RBZ.Projekt.Models;

public class Rating
{
    [Key]
    public int rating_id { get; set; }
    public int RatingInstitutionId { get; set; }
    public RatingInstitution RatingInstitution {get; set;}
    public int MovieId { get; set; }
    public Movie Movie { get; set; }
    public double Value { get; set; }
}

