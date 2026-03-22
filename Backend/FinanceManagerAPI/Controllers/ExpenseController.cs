using FinanceManagerAPI.DTOs;
using FinanceManagerAPI.Extensions;
using FinanceManagerAPI.Models;
using FinanceManagerAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinanceManagerAPI.Controllers;

[ApiController]
[Authorize]
[Route("api/expenses")]
public class ExpenseController : ControllerBase
{
    private readonly IExpenseService _expenseService;

    public ExpenseController(IExpenseService expenseService)
    {
        _expenseService = expenseService;
    }

    /// <summary>
    /// Get all expenses of current user.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var userId = User.GetUserId();
        var expenses = await _expenseService.GetAllAsync(userId);
        return Ok(expenses);
    }

    /// <summary>
    /// Get a single expense by id.
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var userId = User.GetUserId();
        var expense = await _expenseService.GetByIdAsync(id, userId);
        return expense is null ? NotFound() : Ok(expense);
    }

    /// <summary>
    /// Create a new expense. Categories: Food, Travel, Bills, Shopping, Entertainment, Other.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ExpenseDto dto)
    {
        if (!Enum.IsDefined(typeof(ExpenseCategory), dto.Category))
        {
            return BadRequest(new { message = "Invalid expense category." });
        }

        var userId = User.GetUserId();
        var created = await _expenseService.CreateAsync(dto, userId);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>
    /// Update an existing expense transaction.
    /// </summary>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] ExpenseDto dto)
    {
        if (!Enum.IsDefined(typeof(ExpenseCategory), dto.Category))
        {
            return BadRequest(new { message = "Invalid expense category." });
        }

        var userId = User.GetUserId();
        var updated = await _expenseService.UpdateAsync(id, dto, userId);
        return updated is null ? NotFound() : Ok(updated);
    }

    /// <summary>
    /// Delete an expense transaction.
    /// </summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = User.GetUserId();
        var deleted = await _expenseService.DeleteAsync(id, userId);
        return deleted ? NoContent() : NotFound();
    }
}
