using Microsoft.AspNetCore.Mvc;
using POS_Backend.Logica.Domain;
using POS_Backend.Logica.Dtos;

namespace POS_Backend.Controllers;

[ApiController]
[Route("api/asientos")]
public class JournalEntryController : ControllerBase
{
    private readonly JournalEntryDomain _journalEntryDomain;

    public JournalEntryController(JournalEntryDomain journalEntryDomain) =>
        _journalEntryDomain = journalEntryDomain;

    [HttpGet]
    public async Task<ActionResult<List<JournalEntryDto>>> GetAll(
        [FromQuery] DateTime? from, [FromQuery] DateTime? to, CancellationToken ct)
        => Ok(await _journalEntryDomain.GetAllAsync(from, to, ct));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<JournalEntryDto>> GetById(int id, CancellationToken ct)
        => Ok(await _journalEntryDomain.GetByIdAsync(id, ct));

    [HttpPost]
    public async Task<ActionResult<JournalEntryDto>> Create(JournalEntryRequest request, CancellationToken ct)
    {
        var created = await _journalEntryDomain.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPost("{id:int}/anular")]
    public async Task<ActionResult<JournalEntryDto>> Void(int id, VoidJournalEntryRequest request, CancellationToken ct)
    {
        var reversal = await _journalEntryDomain.VoidAsync(id, request, ct);
        return CreatedAtAction(nameof(GetById), new { id = reversal.Id }, reversal);
    }
}