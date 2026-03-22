namespace FinanceManagerAPI.DTOs;

public class MonthlyExpenseReportDto
{
    public int Month { get; set; }

    public int Year { get; set; }

    public decimal TotalAmount { get; set; }
}
