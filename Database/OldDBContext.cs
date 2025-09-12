using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace RBZ.Projekt.Database;

public class OldDbContext : DbContext
{
    public DbSet<OldMovie> Movies { get; set; }
    public DbSet<OldActor> Actors { get; set; }
    public DbSet<OldMovieActor> MovieActors { get; set; }
    public DbSet<OldMovieGenre> MovieGenres { get; set; }
    public DbSet<OldGenre> Genres { get; set; }
    public DbSet<OldDirector> Directors { get; set; }

    public OldDbContext(DbContextOptions<OldDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<OldMovie>().HasKey(m => m.Id);
        modelBuilder.Entity<OldActor>().HasKey(a => a.Id);
        modelBuilder.Entity<OldGenre>().HasKey(g => g.Id);
        modelBuilder.Entity<OldDirector>().HasKey(d => d.Id);

        modelBuilder.Entity<OldMovieActor>()
            .HasKey(ma => new { ma.MovieId, ma.ActorId, ma.Role });

        modelBuilder.Entity<OldMovieGenre>()
            .HasKey(mg => new { mg.MovieId, mg.GenreId });
    }
}


public class OldMovie
{
    [Column("movie_id")]
    public int Id { get; set; }
    [Column("title")]
    public string Title { get; set; }
    [Column("year")]
    public int Year { get; set; }
}

public class OldActor
{
    [Column("actor_id")]
    public int Id { get; set; }
    [Column("name")]
    public string Name { get; set; }
}

public class OldMovieActor
{
    [Column("movie_id")]
    public int MovieId { get; set; }
    [Column("actor_id")]
    public int ActorId { get; set; }
    [Column("role")]
    public string Role { get; set; }
}

public class OldMovieGenre
{
    [Column("movie_id")]
    public int MovieId { get; set; }
    [Column("genre_id")]
    public int GenreId { get; set; }
}

public class OldGenre
{
    [Column("genre_id")]
    public int Id { get; set; }
    [Column("genre_name")]
    public string Name { get; set; }
}

public class OldDirector
{
    [Column("director_id")]
    public int Id { get; set; }
    [Column("name")]
    public string Name { get; set; }
}

