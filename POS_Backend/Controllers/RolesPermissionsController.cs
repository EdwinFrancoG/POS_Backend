using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using POS_Backend.Datos;
using POS_Backend.Datos.Models;

namespace POS_Backend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RolesPermissionsController : ControllerBase
{
    private readonly AppDbContext _context;

    public RolesPermissionsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<RolesPermissions>>> GetRolesPermissions()
    {
        return await _context.RolesPermissions
            .Include(rp => rp.Role)
            .Include(rp => rp.Permission)
            .ToListAsync();
    }

    [HttpGet("{roleId}/{permissionId}")]
    public async Task<ActionResult<RolesPermissions>> GetRolesPermission(int roleId, int permissionId)
    {
        var rolesPermission = await _context.RolesPermissions
            .Include(rp => rp.Role)
            .Include(rp => rp.Permission)
            .FirstOrDefaultAsync(rp => rp.RoleId == roleId &&
                                       rp.PermissionId == permissionId);

        if (rolesPermission == null) return NotFound();

        return rolesPermission;
    }

    [HttpPost]
    public async Task<ActionResult<RolesPermissions>> PostRolesPermission(RolesPermissions rolesPermission)
    {
        _context.RolesPermissions.Add(rolesPermission);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetRolesPermission),
            new { roleId = rolesPermission.RoleId, permissionId = rolesPermission.PermissionId },
            rolesPermission);
    }

    [HttpDelete("{roleId}/{permissionId}")]
    public async Task<IActionResult> DeleteRolesPermission(int roleId, int permissionId)
    {
        var rolesPermission = await _context.RolesPermissions
            .FindAsync(roleId, permissionId);

        if (rolesPermission == null) return NotFound();

        _context.RolesPermissions.Remove(rolesPermission);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}