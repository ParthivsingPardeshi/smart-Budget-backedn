using FinanceManagerAPI.Models;

namespace FinanceManagerAPI.Repositories;

public interface IExpenseRepository
{
    Task<List<Expense>> GetByUserIdAsync(int userId);
    Task<Expense?> GetByIdAsync(int id, int userId);
    Task<Expense> CreateAsync(Expense expense);
    Task UpdateAsync(Expense expense);
    Task DeleteAsync(Expense expense);
}
