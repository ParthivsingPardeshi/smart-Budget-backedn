using FinanceManagerAPI.DTOs;
using FinanceManagerAPI.Models;
using FinanceManagerAPI.Repositories;

namespace FinanceManagerAPI.Services;

public class ExpenseService : IExpenseService
{
    private readonly IExpenseRepository _expenseRepository;

    public ExpenseService(IExpenseRepository expenseRepository)
    {
        _expenseRepository = expenseRepository;
    }

    public Task<List<Expense>> GetAllAsync(int userId)
    {
        return _expenseRepository.GetByUserIdAsync(userId);
    }

    public Task<Expense?> GetByIdAsync(int id, int userId)
    {
        return _expenseRepository.GetByIdAsync(id, userId);
    }

    public Task<Expense> CreateAsync(ExpenseDto dto, int userId)
    {
        var expense = new Expense
        {
            UserId = userId,
            Category = dto.Category,
            Amount = dto.Amount,
            Date = dto.Date,
            Description = dto.Description?.Trim()
        };

        return _expenseRepository.CreateAsync(expense);
    }

    public async Task<Expense?> UpdateAsync(int id, ExpenseDto dto, int userId)
    {
        var existing = await _expenseRepository.GetByIdAsync(id, userId);
        if (existing is null)
        {
            return null;
        }

        existing.Category = dto.Category;
        existing.Amount = dto.Amount;
        existing.Date = dto.Date;
        existing.Description = dto.Description?.Trim();

        await _expenseRepository.UpdateAsync(existing);
        return existing;
    }

    public async Task<bool> DeleteAsync(int id, int userId)
    {
        var existing = await _expenseRepository.GetByIdAsync(id, userId);
        if (existing is null)
        {
            return false;
        }

        await _expenseRepository.DeleteAsync(existing);
        return true;
    }
}
