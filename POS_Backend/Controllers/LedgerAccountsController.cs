

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using POS_Backend.Datos;
using POS_Backend.Datos.Models;

namespace POS_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LedgerAccountsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public LedgerAccountsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/LedgerAccounts
        [HttpGet]
        public async Task<IActionResult> GetLedgerAccounts()
        {
            var accounts = await _context.Set<LedgerAccount>()
                .ToListAsync();

            return Ok(accounts);
        }

        // GET: api/LedgerAccounts/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetLedgerAccount(int id)
        {
            var account = await _context.Set<LedgerAccount>()
                .FindAsync(id);

            if (account == null)
                return NotFound();

            return Ok(account);
        }

        // POST: api/LedgerAccounts
        [HttpPost]
        public async Task<IActionResult> PostLedgerAccount(
            [FromBody] LedgerAccount account)
        {
            _context.Set<LedgerAccount>().Add(account);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetLedgerAccount),
                new { id = account.Id },
                account);
        }

        // PUT: api/LedgerAccounts/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLedgerAccount(
            int id, [FromBody] LedgerAccount account)
        {
            if (id != account.Id)
                return BadRequest();

            var existingAccount = await _context
                .Set<LedgerAccount>().FindAsync(id);

            if (existingAccount == null)
                return NotFound();

            existingAccount.Code = account.Code;
            existingAccount.Name = account.Name;
            existingAccount.Type = account.Type;
            existingAccount.Nature = account.Nature;
            existingAccount.Level = account.Level;
            existingAccount.AllowsPosting = account.AllowsPosting;
            existingAccount.IsActive = account.IsActive;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/LedgerAccounts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLedgerAccount(int id)
        {
            var account = await _context.Set<LedgerAccount>()
                .FindAsync(id);

            if (account == null)
                return NotFound();

            account.IsActive = false;

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
