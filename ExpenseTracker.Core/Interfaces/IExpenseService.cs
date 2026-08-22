using ExpenseTracker.Core.Models;

namespace ExpenseTracker.Core.Interfaces;

public interface IExpenseService
{
    Task AddExpenseAsync(
        Expense expense,
        string userId);

    Task<List<Expense>> GetExpensesAsync(
        string userId);

    Task MarkAsPaidAsync(
        int id,
        string userId);

    Task DeleteAsync(
        int id,
        string userId);

    Task UpdateAsync(
        Expense expense,
        string userId);

    Task<List<Expense>> GetByMonthAsync(
        int month,
        string userId);
}