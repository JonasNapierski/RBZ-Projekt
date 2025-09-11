using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Web;
using RBZ.Projekt.Database;
using RBZ.Projekt.Models;

public class dbMigration
{
	private readonly SQLiteContext _context;
	private readonly OldDbContext _oldContext;

	public dbMigration(SQLiteContext context, OldDbContext oldContext)
	{
		_context = context;
		_oldContext = oldContext;
	}

	public void translateOldDbDataToNew()
	{
		//movies
		foreach (var oldMovie in _oldContext.Movies.ToList())
		{
			_context.Movie.Add(new Movie
			{
				MovieId = oldMovie.Id,
				Title = oldMovie.Title,
				Year = oldMovie.Year
			});
		}
		_context.SaveChanges();

		//actors
		foreach (var oldActor in _oldContext.Actors.ToList())
		{
			string[] parts = oldActor.Name.Split(' ');
			_context.Actor.Add(new Actor
			{
				Id = oldActor.Id,
				FirstName = parts[0],
				LastName = parts[1]
			});
		}
		_context.SaveChanges();

		//genres
		foreach (var oldGenre in _oldContext.Genres.ToList())
		{
			_context.Genre.Add(new Genre
			{
				Id = oldGenre.Id,
				Name = oldGenre.Name
			});
		}
		_context.SaveChanges();

		//movieGenre
		foreach (var oldMovieGenre in _oldContext.MovieGenres.ToList())
		{
			bool movieExists = _context.Movie.Any(m => m.MovieId == oldMovieGenre.MovieId);
			bool genreExists = _context.Genre.Any(g => g.Id == oldMovieGenre.GenreId);

			if (movieExists && genreExists)
			{
				_context.MovieGenre.Add(new MovieGenre
				{
					MovieId = oldMovieGenre.MovieId,
					GenreId = oldMovieGenre.GenreId
				});
			}
			else
			{
				Console.WriteLine($"Skipped MovieGenre: MovieId={oldMovieGenre.MovieId}, GenreId={oldMovieGenre.GenreId}");
			}
		}
		_context.SaveChanges();

		//roles
		Dictionary<string, int> roles = new Dictionary<string, int>();
		int rolesCounter = 1;

		foreach (var oldMovieActor in _oldContext.MovieActors)
		{
			if (!roles.ContainsKey(oldMovieActor.Role))
			{
				roles.Add(oldMovieActor.Role, rolesCounter);
				_context.Role.Add(new Role
				{
					Id = rolesCounter,
					Name = oldMovieActor.Role
				});
				rolesCounter++;
			}
		}
		_context.SaveChanges();

		//movieActor
		foreach (var oldMovieActor in _oldContext.MovieActors)
		{
			bool movieExists = _context.Movie.Any(m => m.MovieId == oldMovieActor.MovieId);
			bool actorExists = _context.Actor.Any(a => a.Id == oldMovieActor.ActorId);

			if (movieExists && actorExists)
			{
				_context.MovieActor.Add(new MovieActor
				{
					MovieId = oldMovieActor.MovieId,
					ActorId = oldMovieActor.ActorId,
					RoleId = roles[oldMovieActor.Role]
				});
			}
			else
			{
				Console.WriteLine($"Skipped MovieActor: MovieId={oldMovieActor.MovieId}, ActorId={oldMovieActor.ActorId}");
			}
		}
		_context.SaveChanges();
	}
}