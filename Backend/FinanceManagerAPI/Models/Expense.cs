using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinanceManagerAPI.Models;

public enum ExpenseCategory
{
    Food = 1,
    Travel = 2,
    Bills = 3,
    Shopping = 4,
    Entertainment = 5,
    Other = 6
}

public class Expense
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public ExpenseCategory Category { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }

    public DateTime Date { get; set; } = DateTime.UtcNow;

    [MaxLength(500)]
    public string? Description { get; set; }

    public User? User { get; set; }
}
