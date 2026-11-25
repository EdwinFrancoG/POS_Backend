using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using POS_Backend.Datos;
using POS_Backend.Datos.Models;

namespace POS_Backend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserRolesController : ControllerBase
{
    private readonly AppDbContext _context;

    public UserRolesController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/UserRoles
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserRoles>>> GetUserRoles()
    {
        return await _context.UserRoles
            .Include(ur => ur.User)
            .Include(ur => ur.Role)
            .ToListAsync();
    }

    // GET: api/UserRoles/5/2
    [HttpGet("{userId}/{roleId}")]
    public async Task<ActionResult<UserRoles>> GetUserRole(int userId, int roleId)
    {
        var userRole = await _context.UserRoles
            .FirstOrDefaultAsync(ur =>
                ur.UserId == userId && ur.RoleId == roleId);

        if (userRole == null)
            return NotFound();

        return userRole;
    }

    // POST: api/UserRoles
    [HttpPost]
    public async Task<ActionResult<UserRoles>> PostUserRole(UserRoles userRole)
    {
        _context.UserRoles.Add(userRole);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetUserRole),
            new { userId = userRole.UserId, roleId = userRole.RoleId },
            userRole);
    }

    // DELETE: api/UserRoles/5/2
    [HttpDelete("{userId}/{roleId}")]
    public async Task<IActionResult> DeleteUserRole(int userId, int roleId)
    {
        var userRole = await _context.UserRoles
            .FindAsync(userId, roleId);

        if (userRole == null)
            return NotFound();

        _context.UserRoles.Remove(userRole);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}