using FinanceManagerAPI.DTOs;
using FinanceManagerAPI.Models;

namespace FinanceManagerAPI.Services;

public interface IIncomeService
{
    Task<List<Income>> GetAllAsync(int userId);
    Task<Income?> GetByIdAsync(int id, int userId);
    Task<Income> CreateAsync(IncomeDto dto, int userId);
    Task<Income?> UpdateAsync(int id, IncomeDto dto, int userId);
    Task<bool> DeleteAsync(int id, int userId);
}
