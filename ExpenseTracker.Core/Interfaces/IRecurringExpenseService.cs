using ExpenseTracker.Core.Models;

namespace ExpenseTracker.Core.Interfaces;

public interface IRecurringExpenseService
{
    Task AddAsync(
        RecurringExpense recurringExpense,
        string userId);

    Task<List<RecurringExpense>> GetAllAsync(
        string userId);

    Task<List<RecurringExpense>> GetActiveAsync(
        string userId);
    Task<List<RecurringExpense>> GetActiveForBackgroundAsync();

    Task UpdateAsync(
        RecurringExpense recurringExpense,
        string userId);

    Task<RecurringExpense?> GetByIdAsync(
        int id,
        string userId);

    Task GenerateExpenseAsync(
        int id);

    Task DeleteAsync(
        int id,
        string userId);
}