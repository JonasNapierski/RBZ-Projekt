using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RBZ.Projekt.Database;
using RBZ.Projekt.Models;
using RBZ.Projekt.Models.REST;

namespace RBZ.Projekt.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CountryController : ControllerBase
{
    protected readonly SQLiteContext _context;

    public CountryController(SQLiteContext context) 
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Country>>> GetCountries()
    {
        return await _context.Countries.Include(c => c.Currency).ToListAsync();
    }

    
    [HttpGet("{id}")]
    public ActionResult<Country> GetCountryById(int id)
    {
        if (id <= 0)
        {
            return BadRequest();
        }


        Country? country = _context.Countries.Include(c => c.Currency).FirstOrDefault(c => c.Id == id);

        if (country is null)
        {
            return NotFound();
        }

        return country;
    }

    [HttpPost]
    public IActionResult AddCountry(CountryCreateRequest createCountry)
    {
        
        Country? country = CreateCountry(createCountry);

        if (country is null)
        {
            return BadRequest();
        }

        try
        {
            _context.Countries.Add(country);
            _context.SaveChanges();
        }
        catch
        {
            return BadRequest();
        }

        return Created();
    }


    [HttpDelete("{id}")]
    public IActionResult DeleteCountry(int id)
    {

        if (id <= 0)
        {
            return BadRequest();
        }

        var country = _context.Countries.Find(id);

        if (country is null)
        {
            return NotFound();
        }

        _context.Countries.Remove(country);
        _context.SaveChanges();

        return NoContent();
    }


    [HttpPut]
    public ActionResult<Country> UpdateCountry(CountryPatchRequest updateCountry)
    {
        if (updateCountry is null)
        {
            return BadRequest();
        }

        string name = updateCountry?.Name?? string.Empty;
        Country? country = _context.Countries.Find(updateCountry);
        if (country is null) 
        {
            if (string.IsNullOrWhiteSpace(updateCountry?.CurrencySymbol) is false){
                CountryCreateRequest createRequest = new ()
                {
                    Name = name,
                    CurrencySymbol = updateCountry.CurrencySymbol,

                };

                country = CreateCountry(createRequest);

                if (country is null)
                {
                    return BadRequest();
                }
            }else{
                return BadRequest();
            }
                
        }
        else
        {
            if (updateCountry?.Name is not null)
            {
                country.Name = name;
            }

            if (string.IsNullOrWhiteSpace(updateCountry?.CurrencySymbol) is false)
            {
                Currency? currency = _context.Currencies.Where(c => string.Equals(c.Symbol, updateCountry.CurrencySymbol)).FirstOrDefault();
                if (currency is not null)
                {
                    country.Currency = currency;
                    country.CurrencyId = currency.Id;
                }
            }
        }


        return country;
    }
    
    private Country? CreateCountry(CountryCreateRequest createCountry)
    {
        if (string.IsNullOrWhiteSpace(createCountry.Name))
        {
            return null;
        }
        Country? country = null;

        Currency? currency = _context.Currencies.Where(c => string.Equals(c.Symbol, createCountry.CurrencySymbol)).FirstOrDefault();

        if (currency is not null)
        {
            country = new(){
                Name = createCountry.Name,
                CurrencyId = currency.Id,
                Currency = currency,
            };
        }

        return country;
    }
}
