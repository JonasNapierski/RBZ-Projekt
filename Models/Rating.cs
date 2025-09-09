using System.ComponentModel.DataAnnotations;

namespace RBZ.Projekt.Models;

public class Rating
{
    [Key]
    public int rating_id { get; set; }
    public RatingInstitution RatingInstitution {get; set;}
    public Movie Movie {get; set;}
    public long? Budget {get; set;}
}

