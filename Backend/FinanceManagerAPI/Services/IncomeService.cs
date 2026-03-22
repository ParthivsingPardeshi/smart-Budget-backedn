using FinanceManagerAPI.DTOs;
using FinanceManagerAPI.Models;
using FinanceManagerAPI.Repositories;

namespace FinanceManagerAPI.Services;

public class IncomeService : IIncomeService
{
    private readonly IIncomeRepository _incomeRepository;

    public IncomeService(IIncomeRepository incomeRepository)
    {
        _incomeRepository = incomeRepository;
    }

    public Task<List<Income>> GetAllAsync(int userId)
    {
        return _incomeRepository.GetByUserIdAsync(userId);
    }

    public Task<Income?> GetByIdAsync(int id, int userId)
    {
        return _incomeRepository.GetByIdAsync(id, userId);
    }

    public Task<Income> CreateAsync(IncomeDto dto, int userId)
    {
        var income = new Income
        {
            UserId = userId,
            Source = dto.Source.Trim(),
            Amount = dto.Amount,
            Date = dto.Date,
            Notes = dto.Notes?.Trim()
        };

        return _incomeRepository.CreateAsync(income);
    }

    public async Task<Income?> UpdateAsync(int id, IncomeDto dto, int userId)
    {
        var existing = await _incomeRepository.GetByIdAsync(id, userId);
        if (existing is null)
        {
            return null;
        }

        existing.Source = dto.Source.Trim();
        existing.Amount = dto.Amount;
        existing.Date = dto.Date;
        existing.Notes = dto.Notes?.Trim();

        await _incomeRepository.UpdateAsync(existing);
        return existing;
    }

    public async Task<bool> DeleteAsync(int id, int userId)
    {
        var existing = await _incomeRepository.GetByIdAsync(id, userId);
        if (existing is null)
        {
            return false;
        }

        await _incomeRepository.DeleteAsync(existing);
        return true;
    }
}
