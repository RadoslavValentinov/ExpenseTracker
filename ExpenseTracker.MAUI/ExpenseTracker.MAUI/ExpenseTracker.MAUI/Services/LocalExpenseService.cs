using ExpenseTracker.Core.Models;
using ExpenseTracker.MAUI.Data;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.MAUI.Services;

public class LocalExpenseService
{
    private readonly IDbContextFactory<ExpenseDbContext> _dbFactory;

    public LocalExpenseService(
        IDbContextFactory<ExpenseDbContext> dbFactory)
    {
        _dbFactory = dbFactory;
    }

    public async Task<List<Expense>> GetExpensesAsync()
    {
        await using var db =
            await _dbFactory.CreateDbContextAsync();

        return await db.Expenses
            .OrderByDescending(e => e.Id)
            .ToListAsync();
    }

    public async Task AddTestExpenseAsync()
    {
        await using var db =
            await _dbFactory.CreateDbContextAsync();

        var expense = new Expense
        {
            UserId = "local-user",
            Title = "MAUI SQLite Test",
            Amount = 9.99m,
            DueDate = DateTime.Now.AddDays(7),
            Category = "Test",
            IsPaid = false,
            CreatedDate = DateTime.UtcNow
        };

        db.Expenses.Add(expense);

        await db.SaveChangesAsync();
    }
}