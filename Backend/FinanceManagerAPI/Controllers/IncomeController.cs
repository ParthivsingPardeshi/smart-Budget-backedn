using FinanceManagerAPI.DTOs;
using FinanceManagerAPI.Extensions;
using FinanceManagerAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinanceManagerAPI.Controllers;

[ApiController]
[Authorize]
[Route("api/income")]
public class IncomeController : ControllerBase
{
    private readonly IIncomeService _incomeService;

    public IncomeController(IIncomeService incomeService)
    {
        _incomeService = incomeService;
    }

    /// <summary>
    /// Get all incomes of current user.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var userId = User.GetUserId();
        var incomes = await _incomeService.GetAllAsync(userId);
        return Ok(incomes);
    }

    /// <summary>
    /// Get a single income by id.
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var userId = User.GetUserId();
        var income = await _incomeService.GetByIdAsync(id, userId);
        return income is null ? NotFound() : Ok(income);
    }

    /// <summary>
    /// Create a new income transaction.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] IncomeDto dto)
    {
        var userId = User.GetUserId();
        var created = await _incomeService.CreateAsync(dto, userId);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>
    /// Update an existing income transaction.
    /// </summary>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] IncomeDto dto)
    {
        var userId = User.GetUserId();
        var updated = await _incomeService.UpdateAsync(id, dto, userId);
        return updated is null ? NotFound() : Ok(updated);
    }

    /// <summary>
    /// Delete an income transaction.
    /// </summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = User.GetUserId();
        var deleted = await _incomeService.DeleteAsync(id, userId);
        return deleted ? NoContent() : NotFound();
    }
}
