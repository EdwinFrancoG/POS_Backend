using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using POS_Backend.Datos;
using POS_Backend.Datos.Models;

namespace POS_Backend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PermissionsController : ControllerBase
{
    private readonly AppDbContext _context;

    public PermissionsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Permissions>>> GetPermissions()
    {
        return await _context.Permissions
            .Include(p => p.RolesPermissions)
            .ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Permissions>> GetPermission(int id)
    {
        var permission = await _context.Permissions
            .Include(p => p.RolesPermissions)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (permission == null) return NotFound();

        return permission;
    }

    [HttpPost]
    public async Task<ActionResult<Permissions>> PostPermission(Permissions permission)
    {
        _context.Permissions.Add(permission);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetPermission), new { id = permission.Id }, permission);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutPermission(int id, Permissions permission)
    {
        if (id != permission.Id) return BadRequest();

        _context.Entry(permission).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePermission(int id)
    {
        var permission = await _context.Permissions.FindAsync(id);

        if (permission == null) return NotFound();

        _context.Permissions.Remove(permission);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}