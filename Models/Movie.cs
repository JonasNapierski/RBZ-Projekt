namespace RBZ.Projekt.Models;

public class Movie
{
    public int movie_id {get; set;}
    public string title {get; set;}
    public long? budget {get; set;}
    public long? revenue_international {get; set;}
    public long? revenue_domestic {get; set;}
    public int year {get; set;}
    public Country Country {get; set;}
}

