using ExpenseTracker.Core.Models;

namespace ExpenseTracker.Core.Interfaces;

public interface IExpenseService
{
    Task AddExpenseAsync(Expense expense);
    Task<List<Expense>> GetExpensesAsync();
    Task MarkAsPaidAsync(int id);
    Task DeleteAsync(int id);
    Task UpdateAsync(Expense expense);
    Task<List<Expense>> GetByMonthAsync(int month);
}