namespace RBZ.Projekt.Models;

public class MovieFestival
{
    public int Id {get; set;}
    public Festival Festival {get; set;}
    public Category Category {get; set;}
    public CategoryStatus CategoryStatus {get; set;}
}
