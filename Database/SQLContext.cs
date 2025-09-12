using RBZ.Projekt.Models;
using Microsoft.EntityFrameworkCore;

namespace RBZ.Projekt.Database;

public class SQLiteContext : DbContext
{
    public SQLiteContext(DbContextOptions<SQLiteContext> options) : base(options)
    {
    }

    public DbSet<Actor> Actors { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<CategoryStatus> CategoryStatus { get; set; }
    public DbSet<Country> Countries { get; set; }
    public DbSet<Currency> Currencies { get; set; }
    public DbSet<Festival> Festivals { get; set; }
    public DbSet<Genre> Genres { get; set; }
    public DbSet<Movie> Movies { get; set; }
    public DbSet<MovieActor> MovieActors { get; set; }
    public DbSet<MovieFestival> MovieFestivals { get; set; }
    public DbSet<MovieGenre> MovieGenres { get; set; }
    public DbSet<Rating> Ratings { get; set; }
    public DbSet<RatingInstitution> RatingInstitutions { get; set; }
    public DbSet<Role> Roles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<MovieActor>()
            .HasKey(ma => new { ma.MovieId, ma.ActorId, ma.RoleId });

        modelBuilder.Entity<MovieGenre>()
            .HasKey(mg => new { mg.MovieId, mg.GenreId });
    }

    public Country? GetCountry(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return null;
        }

        return Countries.FirstOrDefault(e => string.Equals(e.Name.ToLower(), name.ToLower()));
    }
}
