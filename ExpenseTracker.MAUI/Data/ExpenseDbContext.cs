using ExpenseTracker.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.MAUI.Data;

public class ExpenseDbContext : DbContext
{
    public DbSet<Expense> Expenses => Set<Expense>();
    public DbSet<TaskItem> Tasks => Set<TaskItem>();
    public DbSet<Reminder> Reminders => Set<Reminder>();
    public DbSet<RecurringExpense> RecurringExpenses => Set<RecurringExpense>();

    public ExpenseDbContext(DbContextOptions<ExpenseDbContext> options)
        : base(options)
    {
    }
}