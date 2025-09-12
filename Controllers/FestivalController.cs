using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RBZ.Projekt.Database;
using RBZ.Projekt.Models;
using RBZ.Projekt.Models.REST;

namespace RBZ.Projekt.Controllers;

[Route("api/[controller]")]
[ApiController]
public class FestivalController : ControllerBase
{
    protected readonly SQLiteContext _context;

    public FestivalController(SQLiteContext context) 
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Festival>>> GetFestivals()
    {
        return await _context.Festivals.ToListAsync();
    }

    
    [HttpGet("{id}")]
    public ActionResult<Festival> GetFestivalById(int id)
    {
        if (id <= 0)
        {
            return BadRequest();
        }


        Festival? festival = _context.Festivals.Find(id);

        if (festival is null)
        {
            return NotFound();
        }

        return festival;
    }

    [HttpPost]
    public IActionResult AddFestival(FestivalCreateRequest createFestival)
    {
        
        Festival? festival = CreateFestival(createFestival);

        if (festival is null)
        {
            return BadRequest();
        }

        try
        {
            _context.Festivals.Add(festival);
            _context.SaveChanges();
        }
        catch
        {
            return BadRequest();
        }

        return Created();
    }


    [HttpDelete("{id}")]
    public IActionResult DeleteFestival(int id)
    {

        if (id <= 0)
        {
            return BadRequest();
        }

        var festival = _context.Festivals.Find(id);

        if (festival is null)
        {
            return NotFound();
        }

        _context.Festivals.Remove(festival);
        _context.SaveChanges();

        return NoContent();
    }


    [HttpPut]
    public ActionResult<Festival> UpdateFestival(FestivalPatchRequest updateFestival)
    {
        if (updateFestival is null)
        {
            return BadRequest();
        }

        int movieId = updateFestival?.MovieId ?? -1;
        int Id = updateFestival?.FestivalId ?? -1;
        
        Festival? festival = _context.Festivals.Find(updateFestival?.FestivalId);
        bool hasCreateFestival = festival is null;

        if (festival is null)
        {
            festival = new ();
        }


        if (_context.Movies.TryGet(movieId, out Movie? movie) &&
                movie is not null && updateFestival?.MovieId > 0)
        {
                festival.MovieId = updateFestival.MovieId;
                festival.Movie = movie;
        }
        
        if (string.IsNullOrWhiteSpace(updateFestival?.Name) is false)
        {
            festival.Name = updateFestival.Name;
        }

        if ( updateFestival?.Year > 0)
        {
                festival.Year = updateFestival.Year;
        }
        
        if (string.IsNullOrWhiteSpace(updateFestival?.Location) is  false)
        {
            festival.Location = updateFestival.Location;
        }


        if (hasCreateFestival)
        {
            _context.Festivals.Add(festival);
        }
        else
        {
            _context.Festivals.Update(festival);
        }

        return festival;
    }
    
    private Festival? CreateFestival(FestivalCreateRequest createFestival)
    {

        Festival? festival = null;

        if (_context.Movies.TryGet(createFestival.MovieId , out Movie? movie) && movie is not null)
        {
            festival = new(){
                Year = createFestival.Year,
                Location = createFestival.Location,
                MovieId = createFestival.MovieId,
                Movie = movie,
                Name = createFestival.Name,
            };
        }

        return festival;
    }
}
