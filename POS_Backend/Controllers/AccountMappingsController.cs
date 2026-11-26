using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using POS_Backend.Datos;
using POS_Backend.Datos.Models;

namespace POS_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountMappingsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AccountMappingsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/AccountMappings
        [HttpGet]
        public async Task<IActionResult> GetAccountMappings()
        {
            var mappings = await _context.Set<AccountMapping>()
                .ToListAsync();

            return Ok(mappings);
        }

        // GET: api/AccountMappings/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAccountMapping(int id)
        {
            var mapping = await _context.Set<AccountMapping>()
                .FindAsync(id);

            if (mapping == null)
                return NotFound();

            return Ok(mapping);
        }

        // POST: api/AccountMappings
        [HttpPost]
        public async Task<IActionResult> PostAccountMapping(
            [FromBody] AccountMapping mapping)
        {
            var account = await _context.Set<LedgerAccount>()
                .FindAsync(mapping.LedgerAccountId);

            if (account == null)
                return BadRequest("La cuenta contable no existe.");

            mapping.LedgerAccount = account;

            _context.Set<AccountMapping>().Add(mapping);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetAccountMapping),
                new { id = mapping.Id },
                mapping);
        }

        // PUT: api/AccountMappings/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAccountMapping(
            int id, [FromBody] AccountMapping mapping)
        {
            if (id != mapping.Id)
                return BadRequest();

            var existingMapping = await _context
                .Set<AccountMapping>().FindAsync(id);

            if (existingMapping == null)
                return NotFound();

            var account = await _context.Set<LedgerAccount>()
                .FindAsync(mapping.LedgerAccountId);

            if (account == null)
                return BadRequest("La cuenta contable no existe.");

            existingMapping.Key = mapping.Key;
            existingMapping.LedgerAccountId = mapping.LedgerAccountId;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/AccountMappings/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAccountMapping(int id)
        {
            var mapping = await _context.Set<AccountMapping>()
                .FindAsync(id);

            if (mapping == null)
                return NotFound();

            _context.Set<AccountMapping>().Remove(mapping);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}