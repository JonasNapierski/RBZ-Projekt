using System.ComponentModel.DataAnnotations;

namespace RBZ.Projekt.Models;

public class CategoryStatus
{
    [Key]
    public int Id {get; set;}
    public string Name {get; set;}
}
