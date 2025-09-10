using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RBZ.Projekt.Database;
using RBZ.Projekt.Models;
using RBZ.Projekt.Models.REST;

namespace RBZ.Projekt.Controllers;

[Route("api/[controller]")]
[ApiController]
public class GenreController : ControllerBase
{
    protected readonly SQLiteContext _context;

    public GenreController(SQLiteContext context) 
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Genre>>> GetGenres()
    {
        return await _context.Genres.ToListAsync();
    }

    
    [HttpGet("{id}")]
    public ActionResult<Genre> GetGenreById(int id)
    {
        if (id <= 0)
        {
            return BadRequest();
        }


        Genre? genre = _context.Genres.Find(id);

        if (genre is null)
        {
            return NotFound();
        }

        return genre;
    }

    [HttpPost]
    public IActionResult AddGenre(GenreCreateRequest createGenre)
    {
        
        Genre? genre = CreateGenre(createGenre);

        if (genre is null)
        {
            return BadRequest();
        }

        try
        {
            _context.Genres.Add(genre);
            _context.SaveChanges();
        }
        catch
        {
            return BadRequest();
        }

        return Created();
    }


    [HttpDelete("{id}")]
    public IActionResult DeleteGenre(int id)
    {

        if (id <= 0)
        {
            return BadRequest();
        }

        var genre = _context.Genres.Find(id);

        if (genre is null)
        {
            return NotFound();
        }

        _context.Genres.Remove(genre);
        _context.SaveChanges();

        return NoContent();
    }


    [HttpPut]
    public ActionResult<Genre> UpdateGenre(GenrePatchRequest updateGenre)
    {
        if (updateGenre is null)
        {
            return BadRequest();
        }

        string name = updateGenre?.Name?? string.Empty;
        Genre? genre = _context.Genres.Find(updateGenre?.GenreId);
        if (genre is null)
        {
            GenreCreateRequest createRequest = new ()
            {
                Name = name,
            };

            genre = CreateGenre(createRequest);

            if (genre is null)
            {
                return BadRequest();
            }
                
        }
        else
        {
            if (updateGenre?.Name is not null)
            {
                genre.Name = name;
            }
        }


        return genre;
    }
    
    private Genre? CreateGenre(GenreCreateRequest createGenre)
    {
        if (string.IsNullOrWhiteSpace(createGenre.Name))
        {
            return null;
        }

        Genre genre = new(){
                Name = createGenre.Name,
        };

        return genre;
    }
}
