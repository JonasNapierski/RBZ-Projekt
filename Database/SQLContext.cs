using RBZ.Projekt.Models;
using Microsoft.EntityFrameworkCore;

namespace RBZ.Projekt.Database;

public class SQLiteContext : DbContext
{
    public SQLiteContext(DbContextOptions<SQLiteContext> options) : base(options)
    {
    }

    public DbSet<Actor> Actor { get; set; }
    public DbSet<Category> Category { get; set; }
    public DbSet<CategoryStatus> CategoryStatus { get; set; }
    public DbSet<Country> Country { get; set; }
    public DbSet<Currency> Currency { get; set; }
    public DbSet<Festival> Festival { get; set; }
    public DbSet<Genre> Genre { get; set; }
    public DbSet<Movie> Movie { get; set; }
    public DbSet<MovieActor> MovieActor { get; set; }
    public DbSet<MovieFestival> MovieFestival { get; set; }
    public DbSet<MovieGenre> MovieGenre { get; set; }
    public DbSet<Rating> Rating { get; set; }
    public DbSet<RatingInstitution> RatingInstitution { get; set; }
    public DbSet<Role> Role { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<MovieActor>()
            .HasKey(ma => new { ma.MovieId, ma.ActorId, ma.RoleId });

        modelBuilder.Entity<MovieGenre>()
            .HasKey(mg => new { mg.MovieId, mg.GenreId });
    }
}
