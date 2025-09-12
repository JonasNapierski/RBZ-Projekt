using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RBZ.Projekt.Database;
using RBZ.Projekt.Models;
using RBZ.Projekt.Models.REST;

namespace RBZ.Projekt.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RoleController : ControllerBase
{
    protected readonly SQLiteContext _context;

    public RoleController(SQLiteContext context) 
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Role>>> GetRoles()
    {
        return await _context.Roles.Include(r => r.MovieActors).ToListAsync();
    }

    
    [HttpGet("{id}")]
    public ActionResult<Role> GetRoleById(int id)
    {
        if (id <= 0)
        {
            return BadRequest();
        }


        Role? role = _context.Roles.Find(id);

        if (role is null)
        {
            return NotFound();
        }

        return role;
    }

    [HttpPost]
    public IActionResult AddRole(RoleCreateRequest createRole)
    {
        
        Role? role = CreateRole(createRole);

        if (role is null)
        {
            return BadRequest();
        }

        try
        {
            _context.Roles.Add(role);
            _context.SaveChanges();
        }
        catch
        {
            return BadRequest();
        }

        return Created();
    }


    [HttpDelete("{id}")]
    public IActionResult DeleteRole(int id)
    {

        if (id <= 0)
        {
            return BadRequest();
        }

        var role = _context.Roles.Find(id);

        if (role is null)
        {
            return NotFound();
        }

        _context.Roles.Remove(role);
        _context.SaveChanges();

        return NoContent();
    }


    [HttpPut]
    public ActionResult<Role> UpdateRole(RolePatchRequest updateRole)
    {
        if (updateRole is null)
        {
            return BadRequest();
        }

        string name = updateRole?.Name?? string.Empty;
        Role? role = _context.Roles.Find(updateRole?.RoleId);
        if (role is null)
        {
            RoleCreateRequest createRequest = new ()
            {
                Name = name,
            };

            role = CreateRole(createRequest);

            if (role is null)
            {
                return BadRequest();
            }
                
        }
        else
        {
            if (updateRole?.Name is not null)
            {
                role.Name = name;
            }
        }


        return role;
    }
    
    private Role? CreateRole(RoleCreateRequest createRole)
    {
        if (string.IsNullOrWhiteSpace(createRole.Name))
        {
            return null;
        }

        Role role = new(){
                Name = createRole.Name,
        };

        return role;
    }
}
