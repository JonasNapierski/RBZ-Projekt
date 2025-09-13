using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RBZ.Projekt.Database;
using RBZ.Projekt.Models;
using RBZ.Projekt.Models.REST;

namespace RBZ.Projekt.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CurrencyController : ControllerBase
{
    protected readonly SQLiteContext _context;

    public CurrencyController(SQLiteContext context) 
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Currency>>> GetCurrencies()
    {
        return await _context.Currencies.ToListAsync();
    }

    
    [HttpGet("{id}")]
    public ActionResult<Currency> GetCurrencyById(int id)
    {
        if (id <= 0)
        {
            return BadRequest();
        }


        Currency? currency = _context.Currencies.Find(id);

        if (currency is null)
        {
            return NotFound();
        }

        return currency;
    }

    [HttpPost]
    public IActionResult AddCurrency(CurrencyCreateRequest createCurrency)
    {
        Currency? currency = CreateCurrency(createCurrency);

        if (currency is null)
        {
            return BadRequest();
        }

        try
        {
            _context.Currencies.Add(currency);
            _context.SaveChanges();
        }
        catch
        {
            return BadRequest();
        }

        return Created();
    }


    [HttpDelete("{id}")]
    public IActionResult DeleteCurrency(int id)
    {

        if (id <= 0)
        {
            return BadRequest();
        }

        var currency = _context.Currencies.Find(id);

        if (currency is null)
        {
            return NotFound();
        }

        _context.Currencies.Remove(currency);
        _context.SaveChanges();

        return NoContent();
    }


    [HttpPut]
    public ActionResult<Currency> UpdateCurrency(CurrencyPatchRequest updateCurrency)
    {
        if (updateCurrency is null)
        {
            return BadRequest();
        }

        string symbol = updateCurrency?.Symbol?? string.Empty;
        Currency? currency = _context.Currencies.Find(updateCurrency?.CurrencyId);
        if (currency is null)
        {
            CurrencyCreateRequest createRequest = new ()
            {
                Symbol = symbol,
            };

            currency = CreateCurrency(createRequest);

            if (currency is null)
            {
                return BadRequest();
            }
                
        }
        else
        {
            if (updateCurrency?.Symbol is not null)
            {
                currency.Symbol = symbol;
            }
        }


        return currency;
    }
    
    private Currency? CreateCurrency(CurrencyCreateRequest createCurrency)
    {
        if (string.IsNullOrWhiteSpace(createCurrency.Symbol))
        {
            return null;
        }

        Currency currency = new(){
                Symbol = createCurrency.Symbol
        };

        return currency;
    }
}
