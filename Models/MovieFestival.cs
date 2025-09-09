using System.ComponentModel.DataAnnotations;

namespace RBZ.Projekt.Models;

public class MovieFestival
{
    [Key]
    public int Id {get; set;}
    public Festival Festival {get; set;}
    public Category Category {get; set;}
    public CategoryStatus CategoryStatus {get; set;}
}
