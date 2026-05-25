using ExpenseTracker.Core.Models;

namespace ExpenseTracker.Core.Interfaces;

public interface IRecurringExpenseService
{
    Task AddAsync(RecurringExpense recurringExpense);
    Task<List<RecurringExpense>> GetAllAsync();
    Task<List<RecurringExpense>> GetActiveAsync();
    Task UpdateAsync(RecurringExpense recurringExpense);
}