using FinanceManagerAPI.Data;
using FinanceManagerAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace FinanceManagerAPI.Repositories;

public class ExpenseRepository : IExpenseRepository
{
    private readonly AppDbContext _dbContext;

    public ExpenseRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<Expense>> GetByUserIdAsync(int userId)
    {
        return await _dbContext.Expenses
            .Where(e => e.UserId == userId)
            .OrderByDescending(e => e.Date)
            .ToListAsync();
    }

    public async Task<Expense?> GetByIdAsync(int id, int userId)
    {
        return await _dbContext.Expenses
            .FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId);
    }

    public async Task<Expense> CreateAsync(Expense expense)
    {
        _dbContext.Expenses.Add(expense);
        await _dbContext.SaveChangesAsync();
        return expense;
    }

    public async Task UpdateAsync(Expense expense)
    {
        _dbContext.Expenses.Update(expense);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(Expense expense)
    {
        _dbContext.Expenses.Remove(expense);
        await _dbContext.SaveChangesAsync();
    }
}
