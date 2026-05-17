using ExpenseTracker.Core.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Expense> Expenses => Set<Expense>();
        public DbSet<TaskItem> Tasks => Set<TaskItem>();
        public DbSet<Reminder> Reminders => Set<Reminder>();
        public DbSet<RecurringExpense> RecurringExpenses => Set<RecurringExpense>();

    }
}
