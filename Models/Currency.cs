using System.ComponentModel.DataAnnotations;

namespace RBZ.Projekt.Models;

public class Currency
{
    [Key]
    public int Id {get; set;}
    public string? Symbol {get; set;}
}
