using System;
using System.ComponentModel.DataAnnotations;

namespace finance_backend.Models
{
    public class Transaction
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Title { get; set; } = string.Empty;
        public double Amount { get; set; }
        public DateTime Date { get; set; }
        public string Category { get; set; } = string.Empty;
        public string Type { get; set; } = "expense"; // "expense" or "income"
        public string UserId { get; set; } = string.Empty;
    }
}
