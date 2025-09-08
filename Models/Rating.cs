namespace RBZ.Projekt.Models;

public class Rating
{
    public RatingInstitution RatingInstitution {get; set;}
    public Movie Movie {get; set;}
    public long? Budget {get; set;}
}

