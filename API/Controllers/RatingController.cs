using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RBZ.Projekt.Database;
using RBZ.Projekt.Models;
using RBZ.Projekt.Models.REST;

namespace RBZ.Projekt.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RatingController : ControllerBase
{
    protected readonly SQLiteContext _context;

    public RatingController(SQLiteContext context) 
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Rating>>> GetRatings()
    {
        return await _context.Ratings.ToListAsync();
    }

    
    [HttpGet("{id}")]
    public ActionResult<Rating> GetRatingById(int id)
    {
        if (id <= 0)
        {
            return BadRequest();
        }


        Rating? rating = _context.Ratings.Find(id);

        if (rating is null)
        {
            return NotFound();
        }

        return rating;
    }

    [HttpPost]
    public IActionResult AddRating(RatingCreateRequest createRating)
    {
        
        Rating? rating = CreateRating(createRating);

        if (rating is null)
        {
            return BadRequest();
        }

        try
        {
            _context.Ratings.Add(rating);
            _context.SaveChanges();
        }
        catch
        {
            return BadRequest();
        }

        return Created();
    }


    [HttpDelete("{id}")]
    public IActionResult DeleteRating(int id)
    {

        if (id <= 0)
        {
            return BadRequest();
        }

        var rating = _context.Ratings.Find(id);

        if (rating is null)
        {
            return NotFound();
        }

        _context.Ratings.Remove(rating);
        _context.SaveChanges();

        return NoContent();
    }


    [HttpPut]
    public ActionResult<Rating> UpdateRating(RatingPatchRequest updateRating)
    {
        if (updateRating is null)
        {
            return BadRequest();
        }

        int movieId = updateRating?.MovieId ?? -1;
        int institutionId = updateRating?.RatingInstitutionId ?? -1;
        
        Rating? rating = _context.Ratings.Find(updateRating?.RatingId);
        bool hasCreateRating = rating is null;

        if (rating is null)
        {
            rating = new ();
        }


        if (_context.Movies.TryGet(movieId, out Models.Movie? movie) &&
                movie is not null &&
            _context.RatingInstitutions.TryGet(institutionId, out RatingInstitution? institution) &&
            institution is not null)
        {
            rating.MovieId = movieId;
            rating.Movie = movie;
            rating.RatingInstitutionId = institutionId;
            rating.RatingInstitution = institution;
        }


        if (hasCreateRating)
        {
            _context.Ratings.Add(rating);
        }
        else
        {
            _context.Ratings.Update(rating);
        }

        return rating;
    }
    
    private Rating? CreateRating(RatingCreateRequest createRating)
    {

        Rating? rating = null;

        if (_context.Movies.TryGet(createRating.MovieId , out Models.Movie? movie) && movie is not null &&
            _context.RatingInstitutions.TryGet(createRating.RatingInstitutionId, out RatingInstitution? institution) && institution is not null)
        {
            rating = new(){
                RatingInstitutionId = createRating.RatingInstitutionId,
                RatingInstitution = institution,
                MovieId = createRating.MovieId,
                Movie = movie,
                Value = createRating.Rating
            };
        }

        return rating;
    }
}
