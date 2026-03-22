using FinanceManagerAPI.Data;
using FinanceManagerAPI.DTOs;
using FinanceManagerAPI.Extensions;
using FinanceManagerAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinanceManagerAPI.Controllers;

[ApiController]
[Authorize]
[Route("api/budget")]
public class BudgetController : ControllerBase
{
    private readonly AppDbContext _dbContext;

    public BudgetController(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// Get budget summary for a month/year. Defaults to current month.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] int? month, [FromQuery] int? year)
    {
        var userId = User.GetUserId();
        var now = DateTime.UtcNow;
        var targetMonth = month ?? now.Month;
        var targetYear = year ?? now.Year;

        var budget = await _dbContext.Budgets
            .FirstOrDefaultAsync(b => b.UserId == userId && b.Month == targetMonth && b.Year == targetYear);

        var totalSpent = await _dbContext.Expenses
            .Where(e => e.UserId == userId && e.Date.Month == targetMonth && e.Date.Year == targetYear)
            .SumAsync(e => (decimal?)e.Amount) ?? 0m;

        var result = new BudgetStatusDto
        {
            IsBudgetSet = budget is not null,
            BudgetLimit = budget?.MonthlyLimit ?? 0m,
            TotalSpent = totalSpent,
            RemainingAmount = (budget?.MonthlyLimit ?? 0m) - totalSpent,
            Month = targetMonth,
            Year = targetYear
        };

        return Ok(result);
    }

    /// <summary>
    /// Create budget for a month/year.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] BudgetDto dto)
    {
        var userId = User.GetUserId();
        var existing = await _dbContext.Budgets
            .AnyAsync(b => b.UserId == userId && b.Month == dto.Month && b.Year == dto.Year);

        if (existing)
        {
            return BadRequest(new { message = "Budget already exists for selected month/year. Use PUT to update." });
        }

        var budget = new Budget
        {
            UserId = userId,
            MonthlyLimit = dto.MonthlyLimit,
            Month = dto.Month,
            Year = dto.Year
        };

        _dbContext.Budgets.Add(budget);
        await _dbContext.SaveChangesAsync();

        return Ok(budget);
    }

    /// <summary>
    /// Update budget for a month/year.
    /// </summary>
    [HttpPut]
    public async Task<IActionResult> Update([FromBody] BudgetDto dto)
    {
        var userId = User.GetUserId();
        var budget = await _dbContext.Budgets
            .FirstOrDefaultAsync(b => b.UserId == userId && b.Month == dto.Month && b.Year == dto.Year);

        if (budget is null)
        {
            return NotFound(new { message = "Budget not found for selected month/year." });
        }

        budget.MonthlyLimit = dto.MonthlyLimit;
        await _dbContext.SaveChangesAsync();

        return Ok(budget);
    }
}
