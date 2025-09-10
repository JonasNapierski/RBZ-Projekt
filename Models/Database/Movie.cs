
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RBZ.Projekt.Models
{
    public class Movie
    {
        [Key]
        [Column("movie_id")] 
        public int MovieId { get; set; }

        [Required]
        public string Title { get; set; }

        public long? Budget { get; set; }
        public long? RevenueInternational { get; set; }
        public long? RevenueDomestic { get; set; }
        public int Year { get; set; }

        public int CountryId { get; set; }

        public Country Country { get; set; }

        public ICollection<MovieActor> MovieActors { get; set; }
    }
}

