using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace RBZ.Projekt.Models
{
    public class Role
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string? Name { get; set; }

        public ICollection<MovieActor> MovieActors { get; set; }
    }
}

