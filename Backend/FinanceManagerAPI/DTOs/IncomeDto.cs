using System.ComponentModel.DataAnnotations;

namespace FinanceManagerAPI.DTOs;

public class IncomeDto
{
    [Required]
    [MaxLength(100)]
    public string Source { get; set; } = string.Empty;

    [Range(0.01, double.MaxValue)]
    public decimal Amount { get; set; }

    public DateTime Date { get; set; } = DateTime.UtcNow;

    [MaxLength(500)]
    public string? Notes { get; set; }
}
