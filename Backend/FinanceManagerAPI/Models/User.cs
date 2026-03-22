using System.ComponentModel.DataAnnotations;

namespace FinanceManagerAPI.Models;

public class User
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(150)]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Income> Incomes { get; set; } = new List<Income>();

    public ICollection<Expense> Expenses { get; set; } = new List<Expense>();

    public ICollection<Budget> Budgets { get; set; } = new List<Budget>();
}
