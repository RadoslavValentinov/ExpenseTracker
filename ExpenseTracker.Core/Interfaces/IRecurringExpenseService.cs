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

    // Used by authenticated HTTP requests.
    Task<bool> GenerateExpenseForUserAsync(
        int id,
        string userId);

    // Used only by background processing.
    Task GenerateExpenseAsync(
        int id);

    Task DeleteAsync(
        int id,
        string userId);
}