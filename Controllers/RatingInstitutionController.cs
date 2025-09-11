using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RBZ.Projekt.Database;
using RBZ.Projekt.Models;
using RBZ.Projekt.Models.REST;

namespace RBZ.Projekt.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RatingInstitutionController : ControllerBase
{
    protected readonly SQLiteContext _context;

    public RatingInstitutionController(SQLiteContext context) 
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<RatingInstitution>>> GetRatingInstitutions()
    {
        return await _context.RatingInstitutions.ToListAsync();
    }

    
    [HttpGet("{id}")]
    public ActionResult<RatingInstitution> GetRatingInstitutionById(int id)
    {
        if (id <= 0)
        {
            return BadRequest();
        }


        RatingInstitution? ratingInstitution = _context.RatingInstitutions.Find(id);

        if (ratingInstitution is null)
        {
            return NotFound();
        }

        return ratingInstitution;
    }

    [HttpPost]
    public IActionResult AddRatingInstitution(RatingInstitutionCreateRequest createRatingInstitution)
    {
        
        RatingInstitution? ratingInstitution = CreateRatingInstitution(createRatingInstitution);

        if (ratingInstitution is null)
        {
            return BadRequest();
        }

        try
        {
            _context.RatingInstitutions.Add(ratingInstitution);
            _context.SaveChanges();
        }
        catch
        {
            return BadRequest();
        }

        return Created();
    }


    [HttpDelete("{id}")]
    public IActionResult DeleteRatingInstitution(int id)
    {

        if (id <= 0)
        {
            return BadRequest();
        }

        var ratingInstitution = _context.RatingInstitutions.Find(id);

        if (ratingInstitution is null)
        {
            return NotFound();
        }

        _context.RatingInstitutions.Remove(ratingInstitution);
        _context.SaveChanges();

        return NoContent();
    }


    [HttpPut]
    public ActionResult<RatingInstitution> UpdateRatingInstitution(RatingInstitutionPatchRequest updateRatingInstitution)
    {
        if (updateRatingInstitution is null)
        {
            return BadRequest();
        }

        string name = updateRatingInstitution?.Name?? string.Empty;
        RatingInstitution? ratingInstitution = _context.RatingInstitutions.Find(updateRatingInstitution);
        if (ratingInstitution is null)
        {
            RatingInstitutionCreateRequest createRequest = new ()
            {
                Name = name,
            };

            ratingInstitution = CreateRatingInstitution(createRequest);

            if (ratingInstitution is null)
            {
                return BadRequest();
            }
                
        }
        else
        {
            if (updateRatingInstitution?.Name is not null)
            {
                ratingInstitution.Name = name;
            }
        }


        return ratingInstitution;
    }
    
    private RatingInstitution? CreateRatingInstitution(RatingInstitutionCreateRequest createRatingInstitution)
    {
        if (string.IsNullOrWhiteSpace(createRatingInstitution.Name))
        {
            return null;
        }

        RatingInstitution ratingInstitution = new(){
                Name = createRatingInstitution.Name,
        };

        return ratingInstitution;
    }
}
