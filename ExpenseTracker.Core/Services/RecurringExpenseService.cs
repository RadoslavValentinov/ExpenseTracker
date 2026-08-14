using ExpenseTracker.Core.Interfaces;
using ExpenseTracker.Core.Models;

namespace ExpenseTracker.Core.Services;

public class RecurringExpenseService : IRecurringExpenseService
{
    private static readonly SemaphoreSlim _generationLock = new SemaphoreSlim(1, 1);
    private readonly IRecurringExpenseRepository _repository;
    private readonly IExpenseService _expenseService;

    public RecurringExpenseService(
        IRecurringExpenseRepository repository,
        IExpenseService expenseService)
    {
        _repository = repository;
        _expenseService = expenseService;
    }

    public Task AddAsync(RecurringExpense recurringExpense)
    {
        return _repository.AddAsync(recurringExpense);
    }

    public Task<List<RecurringExpense>> GetAllAsync()
    {
        return _repository.GetAllAsync();
    }

    public async Task GenerateExpenseAsync(int id)
    {
        await _generationLock.WaitAsync();

        try
        {
            var recurringExpense = await _repository.GetByIdAsync(id);

            if (recurringExpense == null)
                return;

            if (!recurringExpense.IsActive)
                return;

            var now = DateTime.Now;

            if (recurringExpense.LastGeneratedDate.HasValue &&
                recurringExpense.LastGeneratedDate.Value.Year == now.Year &&
                recurringExpense.LastGeneratedDate.Value.Month == now.Month)
            {
                return;
            }

            var day = Math.Min(
                recurringExpense.DayOfMonth,
                DateTime.DaysInMonth(now.Year, now.Month));

            var dueDate = new DateTime(
                now.Year,
                now.Month,
                day);

            var expense = new Expense
            {
                Title = recurringExpense.Title,
                Amount = recurringExpense.Amount,
                DueDate = dueDate,
                IsPaid = false,
                Category = "Recurring"
            };

            await _expenseService.AddExpenseAsync(expense);

            recurringExpense.LastGeneratedDate = DateTime.UtcNow;

            await _repository.UpdateAsync(recurringExpense);
        }
        finally
        {
            _generationLock.Release();
        }
    }

    public Task<List<RecurringExpense>> GetActiveAsync()
    {
        return _repository.GetActiveAsync();
    }

    public Task<RecurringExpense?> GetByIdAsync(int id)
    {
        return _repository.GetByIdAsync(id);
    }


    public Task UpdateAsync(RecurringExpense recurringExpense)
    {
        return _repository.UpdateAsync(recurringExpense);
    }
}