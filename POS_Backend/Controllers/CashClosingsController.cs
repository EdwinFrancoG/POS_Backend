using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using POS_Backend.Datos;
using POS_Backend.Datos.Models;

namespace POS_Backend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CashClosingsController : ControllerBase
{
    private readonly AppDbContext _context;

    public CashClosingsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CashClosings>>> GetCashClosings()
    {
        return await _context.CashClosings
            .Include(c => c.User)
            .ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CashClosings>> GetCashClosing(int id)
    {
        var cashClosing = await _context.CashClosings
            .Include(c => c.User)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (cashClosing == null) return NotFound();

        return cashClosing;
    }

    [HttpPost]
    public async Task<ActionResult<CashClosings>> PostCashClosing(CashClosings cashClosing)
    {
        _context.CashClosings.Add(cashClosing);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetCashClosing),
            new { id = cashClosing.Id },
            cashClosing);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutCashClosing(int id, CashClosings cashClosing)
    {
        if (id != cashClosing.Id) return BadRequest();

        _context.Entry(cashClosing).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCashClosing(int id)
    {
        var cashClosing = await _context.CashClosings.FindAsync(id);

        if (cashClosing == null) return NotFound();

        _context.CashClosings.Remove(cashClosing);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}