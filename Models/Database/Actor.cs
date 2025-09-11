using System.ComponentModel.DataAnnotations;

namespace RBZ.Projekt.Models;

public class Actor
{
    [Key]
    public int Id { get; set; }

    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;

    public ICollection<MovieActor> MovieActors { get; set; } = new MovieActor[0];
}
