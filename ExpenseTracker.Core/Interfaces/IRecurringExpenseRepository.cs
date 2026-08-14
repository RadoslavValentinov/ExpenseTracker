using ExpenseTracker.Core.Models;

namespace ExpenseTracker.Core.Interfaces;

public interface IRecurringExpenseRepository
{
    Task AddAsync(RecurringExpense recurringExpense);
    Task<List<RecurringExpense>> GetAllAsync();
    Task<List<RecurringExpense>> GetActiveAsync();
    Task UpdateAsync(RecurringExpense recurringExpense);
    Task<RecurringExpense?> GetByIdAsync(int id);
}