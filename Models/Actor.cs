using System.ComponentModel.DataAnnotations;

namespace RBZ.Projekt.Models;

public class Actor
{
    [Key]
    public int Id { get; set; }

    public string FirstName { get; set; }
    public string LastName { get; set; }

    public ICollection<MovieActor> MovieActors { get; set; }
}
