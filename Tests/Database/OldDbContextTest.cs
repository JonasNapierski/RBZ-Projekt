using Microsoft.EntityFrameworkCore;
using RBZ.Projekt.Database;

namespace API.Tests;

public class OldDbContextTests
{
    private OldDbContext CreateInMemoryDb()
    {
        var options = new DbContextOptionsBuilder<OldDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var context = new OldDbContext(options);
        context.Database.EnsureCreated();
        return context;
    }

    [Fact]
    public void CanInsertMovie()
    {
        using var db = CreateInMemoryDb();

        var movie = new OldMovie { Id = 1, Title = "Inception", Year = 2010 };
        db.Movies.Add(movie);
        db.SaveChanges();

        Assert.Equal(1, db.Movies.Count());
        Assert.Equal("Inception", db.Movies.Single().Title);
    }

    [Fact]
    public void CanInsertActorAndRelateToMovie()
    {
        using var db = CreateInMemoryDb();

        var actor = new OldActor { Id = 1, Name = "Leonardo DiCaprio" };
        var movie = new OldMovie { Id = 1, Title = "Inception", Year = 2010 };
        var relation = new OldMovieActor { MovieId = 1, ActorId = 1, Role = "Cobb" };

        db.Actors.Add(actor);
        db.Movies.Add(movie);
        db.MovieActors.Add(relation);
        db.SaveChanges();

        Assert.Single(db.MovieActors);
        Assert.Equal("Cobb", db.MovieActors.First().Role);
    }

    [Fact]
    public void CanInsertGenreAndLinkToMovie()
    {
        using var db = CreateInMemoryDb();

        var genre = new OldGenre { Id = 1, Name = "Sci-Fi" };
        var movie = new OldMovie { Id = 1, Title = "Inception", Year = 2010 };
        var relation = new OldMovieGenre { MovieId = 1, GenreId = 1 };

        db.Genres.Add(genre);
        db.Movies.Add(movie);
        db.MovieGenres.Add(relation);
        db.SaveChanges();

        Assert.Single(db.MovieGenres);
        Assert.Equal(1, db.MovieGenres.First().GenreId);
    }

    [Fact]
    public void CanInsertDirector()
    {
        using var db = CreateInMemoryDb();

        var director = new OldDirector { Id = 1, Name = "Christopher Nolan" };
        db.Directors.Add(director);
        db.SaveChanges();

        Assert.Single(db.Directors);
        Assert.Equal("Christopher Nolan", db.Directors.First().Name);
    }
}

