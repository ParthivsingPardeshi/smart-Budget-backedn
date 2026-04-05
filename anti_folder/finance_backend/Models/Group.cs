using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace finance_backend.Models
{
    public class Group
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = string.Empty;
        public List<string> MemberNames { get; set; } = new();
        public double TotalSpent { get; set; }
        public string UserId { get; set; } = string.Empty;
    }
}
