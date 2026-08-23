using ExpenseTracker.Core.Models;

namespace ExpenseTracker.Core.Interfaces;

public interface IExpenseService
{
    Task AddExpenseAsync(
        Expense expense,
        string userId);

    Task<List<Expense>> GetExpensesAsync(
        string userId);

    Task<bool> MarkAsPaidAsync(
        int id,
        string userId);

    Task<bool> DeleteAsync(
        int id,
        string userId);

    Task<bool> UpdateAsync(
        Expense expense,
        string userId);

    Task<List<Expense>> GetByMonthAsync(
        int month,
        string userId);
}