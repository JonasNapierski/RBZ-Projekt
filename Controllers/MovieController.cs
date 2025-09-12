using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RBZ.Projekt.Database;
using RBZ.Projekt.Models;
using RBZ.Projekt.Models.REST;

namespace RBZ.Projekt.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MovieController : ControllerBase
{
    protected readonly SQLiteContext _context;

    public MovieController(SQLiteContext context) 
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Movie>>> GetMovies()
    {
        return await _context.Movies.ToListAsync();
    }

    
    [HttpGet("{id}")]
    public ActionResult<Movie> GetById(int id)
    {
        if (id <= 0)
        {
            return BadRequest();
        }


        Movie? movie = _context.Movies.Find(id);

        if (movie is null)
        {
            return NotFound();
        }

        Country? country = _context.Countries.Find(movie.CountryId);

        if (country is not null)
        {
            Currency? currency = _context.Currencies.Find(country.CurrencyId);

            if (currency is not null)
            {
                country.Currency = currency;
            }
            movie.Country = country;
        }

        if (movie.MovieActors is null)
        {
            movie.MovieActors = [];
        }

        return movie;
    }

    [HttpPost]
    public IActionResult AddMovie(MovieCreateRequest createMovie)
    {
        
        Movie? movie = CreateMovie(createMovie);

        if (movie is null)
        {
            return BadRequest();
        }

        try
        {
            _context.Movies.Add(movie);
            _context.SaveChanges();
        }
        catch
        {
            return BadRequest();
        }

        return Created();
    }


    [HttpDelete("{id}")]
    public IActionResult DeleteMovie(int id)
    {

        if (id <= 0)
        {
            return BadRequest();
        }

        var movie = _context.Movies.Find(id);

        if (movie is null)
        {
            return NotFound();
        }

        _context.Movies.Remove(movie);
        _context.SaveChanges();

        return NoContent();
    }


    [HttpPut]
    public ActionResult<Movie> UpdateMovie(MoviePatchRequest updateMovie)
    {
        if (updateMovie is null)
        {
            return BadRequest();
        }

        Movie? movie = _context.Movies.Find(updateMovie?.MovieId);
        if (movie is null)
        {
            if (updateMovie is MovieCreateRequest)
            {
                movie = CreateMovie(updateMovie);

                if (movie is null)
                {
                    return BadRequest();
                }
                
            }
            else
            {
                return BadRequest();
            }
        }
        else
        {
            movie = new();
            Country? country = _context.GetCountry(updateMovie?.Country);

            if (country is null)
            {
                return NotFound();
            }

            movie.Country = country;

        }


        return movie;
    }
    
    private Movie? CreateMovie(MovieCreateRequest createMovie)
    {
        Movie movie = new(){
            RevenueDomestic = createMovie.RevenueDomestic,
            RevenueInternational = createMovie.RevenueInternational,
            Title = createMovie.Title,
            Year = createMovie.Year
        };


        if (string.IsNullOrWhiteSpace(createMovie.Country))
        {
            return null;
        }


        Country? country = _context.Countries.FirstOrDefault(e => e.Name == createMovie.Country);

        if (country is null)
        {
            return null;
        }


        movie.Country = country;

        return movie;
    }
}
