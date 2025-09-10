using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RBZ.Projekt.Database;
using RBZ.Projekt.Models;
using RBZ.Projekt.Models.REST;

namespace RBZ.Projekt.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ActorController : ControllerBase
{
    protected readonly SQLiteContext _context;

    public ActorController(SQLiteContext context) 
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Actor>>> GetActors()
    {
        return await _context.Actors.Include(a => a.MovieActors).ToListAsync();
    }

    
    [HttpGet("{id}")]
    public ActionResult<Actor> GetActorById(int id)
    {
        if (id <= 0)
        {
            return BadRequest();
        }


        Actor? actor = _context.Actors.Find(id);

        if (actor is null)
        {
            return NotFound();
        }



        if (actor.MovieActors is null)
        {
            actor.MovieActors = [];
        }

        return actor;
    }

    [HttpPost]
    public IActionResult AddActor(ActorCreateRequest createActor)
    {
        
        Actor? actor = CreateActor(createActor);

        if (actor is null)
        {
            return BadRequest();
        }

        try
        {
            _context.Actors.Add(actor);
            _context.SaveChanges();
        }
        catch
        {
            return BadRequest();
        }

        return Created();
    }


    [HttpDelete("{id}")]
    public IActionResult DeleteActor(int id)
    {

        if (id <= 0)
        {
            return BadRequest();
        }

        var actor = _context.Actors.Find(id);

        if (actor is null)
        {
            return NotFound();
        }

        _context.Actors.Remove(actor);
        _context.SaveChanges();

        return NoContent();
    }


    [HttpPut]
    public ActionResult<Actor> UpdateActor(ActorPatchRequest updateActor)
    {
        if (updateActor is null)
        {
            return BadRequest();
        }

        string firstName = updateActor?.FirstName ?? string.Empty;
        string lastName = updateActor?.LastName ?? string.Empty;

        Actor? actor = _context.Actors.Find(updateActor?.ActorId);
        if (actor is null)
        {
            ActorCreateRequest createRequest = new ()
            {
                FirstName = firstName,
                LastName = lastName
            };

            actor = CreateActor(createRequest);

            if (actor is null)
            {
                return BadRequest();
            }
                
        }
        else
        {
            if (updateActor?.FirstName is not null)
            {
                actor.FirstName = firstName;
            }

            if (updateActor?.LastName is not null)
            {
                actor.LastName = lastName;
            }
        }


        return actor;
    }
    
    private Actor? CreateActor(ActorCreateRequest createActor)
    {
        if (string.IsNullOrWhiteSpace(createActor.FirstName) || string.IsNullOrWhiteSpace(createActor.LastName))
        {
            return null;
        }

        Actor actor = new(){
                FirstName = createActor.FirstName,
                LastName = createActor.LastName 
        };

        return actor;
    }
}
