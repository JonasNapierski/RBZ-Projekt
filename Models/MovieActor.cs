using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations.Schema;

namespace RBZ.Projekt.Models
{
    public class MovieActor
    {
        public int MovieId { get; set; }
        public int ActorId { get; set; }
        public int RoleId { get; set; }

        [ForeignKey(nameof(MovieId))]
        public Movie Movie { get; set; }

        [ForeignKey(nameof(ActorId))]
        public Actor Actor { get; set; }

        [ForeignKey(nameof(RoleId))]
        public Role Role { get; set; }
    }
}
