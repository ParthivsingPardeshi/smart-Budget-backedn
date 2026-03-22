namespace FinanceManagerAPI.DTOs;

public class SavingsTrendReportDto
{
    public int Month { get; set; }

    public int Year { get; set; }

    public decimal TotalIncome { get; set; }

    public decimal TotalExpense { get; set; }

    public decimal Savings { get; set; }
}
