using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using POS_Backend.Datos;
using POS_Backend.Datos.Models;

namespace POS_Backend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuditLogsController : ControllerBase
{
    private readonly AppDbContext _context;

    public AuditLogsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AuditLogs>>> GetAuditLogs()
    {
        return await _context.AuditLogs
            .OrderByDescending(a => a.Date)
            .ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AuditLogs>> GetAuditLog(long id)
    {
        var auditLog = await _context.AuditLogs.FindAsync(id);

        if (auditLog == null) return NotFound();

        return auditLog;
    }

    [HttpPost]
    public async Task<ActionResult<AuditLogs>> PostAuditLog(AuditLogs auditLog)
    {
        _context.AuditLogs.Add(auditLog);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetAuditLog),
            new { id = auditLog.Id },
            auditLog);
    }
}