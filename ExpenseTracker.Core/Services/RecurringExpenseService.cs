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


    public async Task AddAsync(
        RecurringExpense recurringExpense,
        string userId)
    {
        recurringExpense.UserId = userId;

        recurringExpense.Title =
            recurringExpense.Title?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(
                recurringExpense.Title))
        {
            throw new ArgumentException(
                "Title is required.");
        }

        if (recurringExpense.Amount <= 0)
        {
            throw new ArgumentException(
                "Amount must be greater than zero.");
        }

        if (recurringExpense.DayOfMonth < 1 ||
            recurringExpense.DayOfMonth > 31)
        {
            throw new ArgumentException(
                "Day of month must be between 1 and 31.");
        }

        await _repository.AddAsync(
            recurringExpense);
    }


    public Task<List<RecurringExpense>> GetAllAsync(
        string userId)
    {
        return _repository.GetAllAsync(
            userId);
    }


    public Task<List<RecurringExpense>> GetActiveAsync(
        string userId)
    {
        return _repository.GetActiveAsync(
            userId);
    }


    public async Task<RecurringExpense?> GetByIdAsync(
        int id,
        string userId)
    {
        return await _repository.GetByIdAsync(
            id,
            userId);
    }


    public async Task UpdateAsync(
        RecurringExpense recurringExpense,
        string userId)
    {
        var existing =
            await _repository.GetByIdAsync(
                recurringExpense.Id,
                userId);

        if (existing == null)
            return;


        var title =
            recurringExpense.Title?.Trim()
            ?? string.Empty;

        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException(
                "Title is required.");
        }

        if (recurringExpense.Amount <= 0)
        {
            throw new ArgumentException(
                "Amount must be greater than zero.");
        }

        if (recurringExpense.DayOfMonth < 1 ||
            recurringExpense.DayOfMonth > 31)
        {
            throw new ArgumentException(
                "Day of month must be between 1 and 31.");
        }


        existing.Title =
            title;

        existing.Amount =
            recurringExpense.Amount;

        existing.DayOfMonth =
            recurringExpense.DayOfMonth;

        existing.IsActive =
            recurringExpense.IsActive;


        await _repository.UpdateAsync(
            existing);
    }


    // =========================
    // USER GENERATION
    // =========================

    public async Task<bool> GenerateExpenseForUserAsync(
        int id,
        string userId)
    {
        await _generationLock.WaitAsync();

        try
        {
            var recurringExpense =
                await _repository.GetByIdAsync(
                    id,
                    userId);

            if (recurringExpense == null)
                return false;

            return await GenerateExpenseInternalAsync(
                recurringExpense);
        }
        finally
        {
            _generationLock.Release();
        }
    }


    // =========================
    // BACKGROUND GENERATION
    // =========================

    public async Task GenerateExpenseAsync(
        int id)
    {
        await _generationLock.WaitAsync();

        try
        {
            var recurringExpense =
                await FindRecurringExpenseForGeneration(
                    id);

            if (recurringExpense == null)
                return;

            await GenerateExpenseInternalAsync(
                recurringExpense);
        }
        finally
        {
            _generationLock.Release();
        }
    }


    private async Task<bool> GenerateExpenseInternalAsync(
        RecurringExpense recurringExpense)
    {
        if (!recurringExpense.IsActive)
            return true;


        var today =
            DateTime.Now.Date;


        // Already generated for current month.
        if (recurringExpense.LastGeneratedDate.HasValue &&
            recurringExpense.LastGeneratedDate.Value.Year ==
                today.Year &&
            recurringExpense.LastGeneratedDate.Value.Month ==
                today.Month)
        {
            return true;
        }


        // Calculate actual day for current month.
        var day =
            Math.Min(
                recurringExpense.DayOfMonth,
                DateTime.DaysInMonth(
                    today.Year,
                    today.Month));


        // Not reached yet.
        if (today.Day < day)
            return true;


        // Planned due date.
        var dueDate =
            new DateTime(
                today.Year,
                today.Month,
                day);


        var expense =
            new Expense
            {
                UserId =
                    recurringExpense.UserId,

                Title =
                    recurringExpense.Title,

                Amount =
                    recurringExpense.Amount,

                DueDate =
                    dueDate,

                IsPaid =
                    false,

                Category =
                    "Recurring"
            };


        await _expenseService.AddExpenseAsync(
            expense,
            recurringExpense.UserId);


        // Remember generation.
        recurringExpense.LastGeneratedDate =
            DateTime.UtcNow;

        await _repository.UpdateAsync(
            recurringExpense);


        return true;
    }


    private async Task<RecurringExpense?>
        FindRecurringExpenseForGeneration(
            int id)
    {
        // Background generation is not tied
        // to an HTTP/JWT user.

        return await _repository
            .GetByIdForBackgroundAsync(id);
    }


    public Task DeleteAsync(
        int id,
        string userId)
    {
        return _repository.DeleteAsync(
            id,
            userId);
    }


    public Task<List<RecurringExpense>>
        GetActiveForBackgroundAsync()
    {
        return _repository
            .GetActiveForBackgroundAsync();
    }
}