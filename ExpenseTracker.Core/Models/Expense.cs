using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.Core.Models
{
    public class Expense
    {
        public int Id { get; set; }

        public string Title { get; set; } = null!;

        public decimal Amount { get; set; }

        public DateTime DueDate { get; set; }

        public bool IsPaid { get; set; } = false;

        public string? Category { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}
