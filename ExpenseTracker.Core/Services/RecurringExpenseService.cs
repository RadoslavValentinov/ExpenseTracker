using ExpenseTracker.Core.Interfaces;
using ExpenseTracker.Core.Models;

namespace ExpenseTracker.Core.Services;

public class RecurringExpenseService : IRecurringExpenseService
{
    private readonly IRecurringExpenseRepository _repository;

    public RecurringExpenseService(IRecurringExpenseRepository repository)
    {
        _repository = repository;
    }

    public Task AddAsync(RecurringExpense recurringExpense)
    {
        return _repository.AddAsync(recurringExpense);
    }

    public Task<List<RecurringExpense>> GetAllAsync()
    {
        return _repository.GetAllAsync();
    }

    public Task<List<RecurringExpense>> GetActiveAsync()
    {
        return _repository.GetActiveAsync();
    }
}