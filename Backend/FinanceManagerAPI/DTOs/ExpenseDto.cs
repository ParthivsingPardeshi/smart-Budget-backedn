using System.ComponentModel.DataAnnotations;
using FinanceManagerAPI.Models;

namespace FinanceManagerAPI.DTOs;

public class ExpenseDto
{
    [Required]
    [EnumDataType(typeof(ExpenseCategory))]
    public ExpenseCategory Category { get; set; }

    [Range(0.01, double.MaxValue)]
    public decimal Amount { get; set; }

    public DateTime Date { get; set; } = DateTime.UtcNow;

    [MaxLength(500)]
    public string? Description { get; set; }
}
