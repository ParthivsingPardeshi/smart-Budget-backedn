using System;
using System.ComponentModel.DataAnnotations;

namespace finance_backend.Models
{
    public class Friend
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public double Balance { get; set; }
        public string UserId { get; set; } = string.Empty;
    }
}
