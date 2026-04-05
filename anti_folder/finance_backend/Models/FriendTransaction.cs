using System;
using System.ComponentModel.DataAnnotations;

namespace finance_backend.Models
{
    public class FriendTransaction
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string FriendId { get; set; } = string.Empty;
        public double Amount { get; set; }
        public string Type { get; set; } = "credit"; // "credit" or "debit"
        public string Description { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string UserId { get; set; } = string.Empty;
    }
}
