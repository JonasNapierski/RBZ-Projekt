using System.ComponentModel.DataAnnotations;

namespace RBZ.Projekt.Models
{
    public class Role
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public ICollection<MovieActor> MovieActors { get; set; } = new MovieActor[0];
    }
}

