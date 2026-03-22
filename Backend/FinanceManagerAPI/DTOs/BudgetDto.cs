using System.ComponentModel.DataAnnotations;

namespace FinanceManagerAPI.DTOs;

public class BudgetDto
{
    [Range(0.01, double.MaxValue)]
    public decimal MonthlyLimit { get; set; }

    [Range(1, 12)]
    public int Month { get; set; }

    [Range(2000, 3000)]
    public int Year { get; set; }
}
