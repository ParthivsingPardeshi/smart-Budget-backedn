namespace FinanceManagerAPI.DTOs;

public class CategoryExpenseReportDto
{
    public string Category { get; set; } = string.Empty;

    public decimal TotalAmount { get; set; }
}
