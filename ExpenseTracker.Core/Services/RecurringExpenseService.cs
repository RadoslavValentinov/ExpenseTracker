using ExpenseTracker.Core.Interfaces;
using ExpenseTracker.Core.Models;

namespace ExpenseTracker.Core.Services;

public class RecurringExpenseService : IRecurringExpenseService
{
    private static readonly SemaphoreSlim _generationLock =
        new SemaphoreSlim(1, 1);

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
            var recurringExpense =
                await _repository.GetByIdAsync(id);

            if (recurringExpense == null)
                return;

            if (!recurringExpense.IsActive)
                return;

            var today = DateTime.Now.Date;

            // If this recurring expense was already
            // generated for the current month, stop.
            if (recurringExpense.LastGeneratedDate.HasValue &&
                recurringExpense.LastGeneratedDate.Value.Year == today.Year &&
                recurringExpense.LastGeneratedDate.Value.Month == today.Month)
            {
                return;
            }

            // Calculate the actual due day for the current month.
            // Example:
            // DayOfMonth = 31
            // February -> 28/29
            var day = Math.Min(
                recurringExpense.DayOfMonth,
                DateTime.DaysInMonth(
                    today.Year,
                    today.Month));

            // If we haven't reached the due day yet,
            // do not generate the expense.
            if (today.Day < day)
                return;

            // The expense keeps the planned due date,
            // even if we generate it later because the
            // application was offline.
            var dueDate = new DateTime(
                today.Year,
                today.Month,
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

            // Remember that this month's expense
            // has already been generated.
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


    public Task DeleteAsync(int id)
    {
        return _repository.DeleteAsync(id);
    }
}