using ExpenseTracker.Core.Models;
using ExpenseTracker.MAUI.Data;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.MAUI.Services;

public class LocalExpenseService
{
    private readonly IDbContextFactory<ExpenseDbContext> _dbFactory;

    public LocalExpenseService(IDbContextFactory<ExpenseDbContext> dbFactory)
    {
        _dbFactory = dbFactory;
    }

    public async Task<List<Expense>> GetExpensesAsync()
    {
        await using var db = await _dbFactory.CreateDbContextAsync();

        return await db.Expenses
            .OrderByDescending(e => e.Id)
            .ToListAsync();
    }

    public async Task<Expense> AddExpenseAsync(
        string title,
        decimal amount,
        DateTime dueDate,
        string? category)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Заглавието е задължително.");

        if (amount < 0)
            throw new ArgumentException("Сумата не може да бъде отрицателна.");

        await using var db = await _dbFactory.CreateDbContextAsync();

        var expense = new Expense
        {
            UserId = "local-user",
            Title = title.Trim(),
            Amount = amount,
            DueDate = dueDate,
            Category = string.IsNullOrWhiteSpace(category)
                ? null
                : category.Trim(),
            IsPaid = false,
            CreatedDate = DateTime.UtcNow
        };

        db.Expenses.Add(expense);

        await db.SaveChangesAsync();

        return expense;
    }

    public async Task MarkAsPaidAsync(int expenseId)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();

        var expense = await db.Expenses
            .FirstOrDefaultAsync(e => e.Id == expenseId);

        if (expense is null)
            return;

        expense.IsPaid = true;

        await db.SaveChangesAsync();
    }

    public async Task DeleteExpenseAsync(int expenseId)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();

        var expense = await db.Expenses
            .FirstOrDefaultAsync(e => e.Id == expenseId);

        if (expense is null)
            return;

        db.Expenses.Remove(expense);

        await db.SaveChangesAsync();
    }

    public async Task UpdateExpenseAsync(
    int expenseId,
    string title,
    decimal amount,
    DateTime dueDate,
    string? category)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Заглавието е задължително.");

        if (amount < 0)
            throw new ArgumentException("Сумата не може да бъде отрицателна.");

        await using var db = await _dbFactory.CreateDbContextAsync();

        var expense = await db.Expenses
            .FirstOrDefaultAsync(e => e.Id == expenseId);

        if (expense is null)
            throw new InvalidOperationException("Разходът не е намерен.");

        expense.Title = title.Trim();
        expense.Amount = amount;
        expense.DueDate = dueDate;
        expense.Category = string.IsNullOrWhiteSpace(category)
            ? null
            : category.Trim();

        await db.SaveChangesAsync();
    }

}