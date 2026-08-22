using ExpenseTracker.Core.Models;

namespace ExpenseTracker.Core.Interfaces;

public interface IExpenseRepository
{
    Task AddAsync(Expense expense);

    Task<List<Expense>> GetAllAsync(
        string userId);

    Task<Expense?> GetByIdAsync(
        int id,
        string userId);

    Task UpdateAsync(Expense expense);

    Task DeleteAsync(
        int id,
        string userId);

    Task<List<Expense>> GetByMonthAsync(
        int month,
        string userId);

    Task MarkAsPaidAsync(
        int id,
        string userId);
}