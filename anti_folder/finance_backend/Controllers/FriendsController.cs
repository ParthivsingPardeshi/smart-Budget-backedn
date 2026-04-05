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
    public class FriendsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public FriendsController(AppDbContext context)
        {
            _context = context;
        }

        private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Friend>>> GetFriends()
        {
            return await _context.Friends.Where(f => f.UserId == UserId).ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<Friend>> PostFriend(Friend friend)
        {
            if (string.IsNullOrEmpty(friend.Id)) friend.Id = Guid.NewGuid().ToString();
            friend.UserId = UserId;
            
            _context.Friends.Add(friend);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetFriends), new { id = friend.Id }, friend);
        }
        
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFriend(string id)
        {
            var friend = await _context.Friends.FirstOrDefaultAsync(f => f.Id == id && f.UserId == UserId);
            if (friend == null) return NotFound();

            _context.Friends.Remove(friend);
            
            var transactions = await _context.FriendTransactions.Where(t => t.FriendId == id && t.UserId == UserId).ToListAsync();
            _context.FriendTransactions.RemoveRange(transactions);
            
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("transactions")]
        public async Task<ActionResult<IEnumerable<FriendTransaction>>> GetAllFriendTransactions()
        {
            return await _context.FriendTransactions.Where(t => t.UserId == UserId).ToListAsync();
        }

        [HttpPost("transactions")]
        public async Task<ActionResult<FriendTransaction>> PostFriendTransaction(FriendTransaction transaction)
        {
            if (string.IsNullOrEmpty(transaction.Id)) transaction.Id = Guid.NewGuid().ToString();
            transaction.UserId = UserId;
            
            _context.FriendTransactions.Add(transaction);
            
            var friend = await _context.Friends.FirstOrDefaultAsync(f => f.Id == transaction.FriendId && f.UserId == UserId);
            if (friend != null)
            {
                if (transaction.Type == "credit") friend.Balance += transaction.Amount;
                else friend.Balance -= transaction.Amount;
            }
            
            await _context.SaveChangesAsync();
            return Ok(transaction);
        }

        [HttpDelete("transactions/{id}")]
        public async Task<IActionResult> DeleteFriendTransaction(string id)
        {
            var transaction = await _context.FriendTransactions.FirstOrDefaultAsync(t => t.Id == id && t.UserId == UserId);
            if (transaction == null) return NotFound();

            var friend = await _context.Friends.FirstOrDefaultAsync(f => f.Id == transaction.FriendId && f.UserId == UserId);
            if (friend != null)
            {
                if (transaction.Type == "credit") friend.Balance -= transaction.Amount;
                else friend.Balance += transaction.Amount;
            }

            _context.FriendTransactions.Remove(transaction);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("{id}/settle")]
        public async Task<IActionResult> SettleFriend(string id)
        {
            var friend = await _context.Friends.FirstOrDefaultAsync(f => f.Id == id && f.UserId == UserId);
            if (friend == null) return NotFound();

            if (friend.Balance != 0) {
                _context.FriendTransactions.Add(new FriendTransaction 
                {
                    Id = Guid.NewGuid().ToString(),
                    FriendId = id,
                    UserId = UserId,
                    Amount = Math.Abs(friend.Balance),
                    Type = friend.Balance > 0 ? "debit" : "credit",
                    Description = "Settlement",
                    Date = DateTime.UtcNow
                });
                friend.Balance = 0;
                await _context.SaveChangesAsync();
            }
            return NoContent();
        }
    }
}
