using System.ComponentModel.DataAnnotations;

namespace RBZ.Projekt.Models;

public class Festival
{
    [Key]
    public int Id {get; set;}
    public string Name {get; set;}
    public int Year {get; set;}
    public string Location {get; set;}
    public int? MovieId { get; set; }
    public Movie? Movie {get; set;}
}
