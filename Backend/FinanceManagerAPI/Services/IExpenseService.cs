using FinanceManagerAPI.DTOs;
using FinanceManagerAPI.Models;

namespace FinanceManagerAPI.Services;

public interface IExpenseService
{
    Task<List<Expense>> GetAllAsync(int userId);
    Task<Expense?> GetByIdAsync(int id, int userId);
    Task<Expense> CreateAsync(ExpenseDto dto, int userId);
    Task<Expense?> UpdateAsync(int id, ExpenseDto dto, int userId);
    Task<bool> DeleteAsync(int id, int userId);
}
