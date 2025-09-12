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
            if (!_context.Movies.Any(m => m.MovieId == oldMovie.Id))
            {
                _context.Movies.Add(new Movie
                {
                    MovieId = oldMovie.Id,
                    Title = oldMovie.Title,
                    Year = oldMovie.Year
                });
            }
        }
		_context.SaveChanges();

		//actors
		foreach (var oldActor in _oldContext.Actors.ToList())
		{
            if (!_context.Actors.Any(a => a.Id == oldActor.Id))
            { 
				string[] parts = oldActor.Name.Split(' ');
				_context.Actors.Add(new Actor
				{
					Id = oldActor.Id,
					FirstName = parts[0],
					LastName = parts[1]
				});
			}
		}
		_context.SaveChanges();

		//genres
		foreach (var oldGenre in _oldContext.Genres.ToList())
		{
            if (!_context.Genres.Any(g => g.Id == oldGenre.Id))
            { 
				_context.Genres.Add(new Genre
				{
					Id = oldGenre.Id,
					Name = oldGenre.Name
				});
			}
		}
		_context.SaveChanges();

        //movieGenre
        foreach (var oldMovieGenre in _oldContext.MovieGenres.ToList())
        {
            if (!_context.Movies.Any(m => m.MovieId == oldMovieGenre.MovieId) ||
                !_context.Genres.Any(g => g.Id == oldMovieGenre.GenreId))
            {
                Console.WriteLine($"Skipped MovieGenre: MovieId={oldMovieGenre.MovieId}, GenreId={oldMovieGenre.GenreId}");
                continue;
            }

            if (_context.MovieGenres.Local.Any(mg => mg.MovieId == oldMovieGenre.MovieId && mg.GenreId == oldMovieGenre.GenreId))
            {
                continue;
            }

            _context.MovieGenres.Add(new MovieGenre
            {
                MovieId = oldMovieGenre.MovieId,
                GenreId = oldMovieGenre.GenreId
            });
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
				_context.Roles.Add(new Role
				{
					Id = rolesCounter,
					Name = oldMovieActor.Role
				});
				rolesCounter++;
			}
		}
		_context.SaveChanges();

        //movieActor
        var newMovieActors = new List<MovieActor>();

        foreach (var oldMovieActor in _oldContext.MovieActors.ToList())
        {
            bool movieExists = _context.Movies.Any(m => m.MovieId == oldMovieActor.MovieId);
            bool actorExists = _context.Actors.Any(a => a.Id == oldMovieActor.ActorId);

            if (movieExists && actorExists)
            {
                bool alreadyTracked = _context.MovieActors.Local
                    .Any(ma => ma.MovieId == oldMovieActor.MovieId &&
                               ma.ActorId == oldMovieActor.ActorId &&
                               ma.RoleId == roles[oldMovieActor.Role]);

                if (alreadyTracked)
                    continue;

                newMovieActors.Add(new MovieActor
                {
                    MovieId = oldMovieActor.MovieId,
                    ActorId = oldMovieActor.ActorId,
                    RoleId = roles[oldMovieActor.Role]
                });
            }
        }

        _context.MovieActors.AddRange(newMovieActors);
        _context.SaveChanges();
    }
}