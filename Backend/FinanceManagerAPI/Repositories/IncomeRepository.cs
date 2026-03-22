using FinanceManagerAPI.Data;
using FinanceManagerAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace FinanceManagerAPI.Repositories;

public class IncomeRepository : IIncomeRepository
{
    private readonly AppDbContext _dbContext;

    public IncomeRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<Income>> GetByUserIdAsync(int userId)
    {
        return await _dbContext.Incomes
            .Where(i => i.UserId == userId)
            .OrderByDescending(i => i.Date)
            .ToListAsync();
    }

    public async Task<Income?> GetByIdAsync(int id, int userId)
    {
        return await _dbContext.Incomes
            .FirstOrDefaultAsync(i => i.Id == id && i.UserId == userId);
    }

    public async Task<Income> CreateAsync(Income income)
    {
        _dbContext.Incomes.Add(income);
        await _dbContext.SaveChangesAsync();
        return income;
    }

    public async Task UpdateAsync(Income income)
    {
        _dbContext.Incomes.Update(income);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(Income income)
    {
        _dbContext.Incomes.Remove(income);
        await _dbContext.SaveChangesAsync();
    }
}
