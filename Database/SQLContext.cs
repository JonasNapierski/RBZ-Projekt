using Microsoft.EntityFrameworkCore;

namespace RBZ.Projekt.Database;

public class SQLiteContext : DbContext 
{
    public SQLiteContext(DbContextOptions options) : base(options)
    {
        DbSet<Actor> Actor {get; set;}
        DbSet<Category> Category {get; set;}
        DbSet<CategoryStatus> CategoryStatus {get; set;}
        DbSet<Country> Country {get; set;}
        DbSet<Currency> Currency {get; set;}
        DbSet<Festival> Festival {get; set;}
        DbSet<Genre> Genre {get; set;}
        DbSet<Movie> Movie {get; set;}
        DbSet<MovieActor> MovieActor {get; set;}
        DbSet<MovieFestival> MovieFestival {get; set;}
        DbSet<MovieGenre> MovieGenre {get; set;}
        DbSet<Rating> Rating {get; set;}
        DbSet<RatingInstitution> RatingInstitution {get; set;}
        DbSet<Role> Role {get; set;}
    }
}
