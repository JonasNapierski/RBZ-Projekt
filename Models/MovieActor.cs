using System.ComponentModel.DataAnnotations;

namespace RBZ.Projekt.Models;

public class MovieActor
{
    [Required]
    public Movie Movie { get; set; }
    [Required]
    public Actor Actor { get; set; }
    [Required]
    public Role Role { get; set; }
}
