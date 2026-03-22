using FinanceManagerAPI.Models;

namespace FinanceManagerAPI.Repositories;

public interface IIncomeRepository
{
    Task<List<Income>> GetByUserIdAsync(int userId);
    Task<Income?> GetByIdAsync(int id, int userId);
    Task<Income> CreateAsync(Income income);
    Task UpdateAsync(Income income);
    Task DeleteAsync(Income income);
}
