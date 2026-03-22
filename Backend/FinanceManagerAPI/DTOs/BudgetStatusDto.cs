namespace FinanceManagerAPI.DTOs;

public class BudgetStatusDto
{
    public bool IsBudgetSet { get; set; }

    public decimal BudgetLimit { get; set; }

    public decimal TotalSpent { get; set; }

    public decimal RemainingAmount { get; set; }

    public int Month { get; set; }

    public int Year { get; set; }
}
