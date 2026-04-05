using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using finance_backend.Data;
using finance_backend.Models;

namespace finance_backend.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class GroupsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public GroupsController(AppDbContext context)
        {
            _context = context;
        }

        private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Group>>> GetGroups()
        {
            return await _context.Groups.Where(g => g.UserId == UserId).ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<Group>> PostGroup(Group group)
        {
            if (string.IsNullOrEmpty(group.Id)) group.Id = Guid.NewGuid().ToString();
            group.UserId = UserId;
            
            _context.Groups.Add(group);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetGroups), new { id = group.Id }, group);
        }

        [HttpPut("{id}/totalspent")]
        public async Task<IActionResult> UpdateGroupTotal(string id, [FromBody] double amount)
        {
            var group = await _context.Groups.FirstOrDefaultAsync(g => g.Id == id && g.UserId == UserId);
            if (group == null) return NotFound();

            group.TotalSpent += amount;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGroup(string id)
        {
            var group = await _context.Groups.FirstOrDefaultAsync(g => g.Id == id && g.UserId == UserId);
            if (group == null) return NotFound();

            _context.Groups.Remove(group);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
