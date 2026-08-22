using ExpenseTracker.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Expense> Expenses { get; set; } = null!;

        public DbSet<TaskItem> Tasks { get; set; } = null!;

        public DbSet<Reminder> Reminders { get; set; } = null!;

        public DbSet<RecurringExpense> RecurringExpenses { get; set; } = null!;
    }
}