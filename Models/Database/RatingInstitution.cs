using System.ComponentModel.DataAnnotations;

namespace RBZ.Projekt.Models;

public class RatingInstitution
{
    [Key]
    public int Id {get; set;}
    public string Name {get; set;} = string.Empty;
}
