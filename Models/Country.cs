using System.ComponentModel.DataAnnotations;

namespace RBZ.Projekt.Models;

public class Country
{
    [Key]
    public int Id {get; set;}
    public string? Name {get; set;}
    public Currency? Currency {get; set;}
}
