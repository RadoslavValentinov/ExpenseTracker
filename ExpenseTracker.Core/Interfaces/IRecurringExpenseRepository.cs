using ExpenseTracker.Core.Models;

namespace ExpenseTracker.Core.Interfaces;

public interface IRecurringExpenseRepository
{
    Task AddAsync(RecurringExpense recurringExpense);

    Task<RecurringExpense?> GetByIdForBackgroundAsync(
    int id);

    Task<List<RecurringExpense>> GetAllAsync(
        string userId);

    Task<List<RecurringExpense>> GetActiveAsync(
        string userId);

    Task UpdateAsync(
        RecurringExpense recurringExpense);

    Task<RecurringExpense?> GetByIdAsync(
        int id,
        string userId);

    Task DeleteAsync(
        int id,
        string userId);

    Task<List<RecurringExpense>> GetActiveForBackgroundAsync();
}