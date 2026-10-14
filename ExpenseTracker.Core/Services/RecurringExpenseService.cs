using ExpenseTracker.Core.Interfaces;
using ExpenseTracker.Core.Models;

namespace ExpenseTracker.Core.Services;

public class RecurringExpenseService : IRecurringExpenseService
{
    private readonly IRecurringExpenseRepository _repository;
    private readonly IExpenseService _expenseService;

    private static readonly SemaphoreSlim _generationLock =
        new(1, 1);

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
        if (recurringExpense == null)
            throw new ArgumentNullException(
                nameof(recurringExpense));

        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException(
                "User id is required.",
                nameof(userId));

        if (string.IsNullOrWhiteSpace(
                recurringExpense.Title))
        {
            throw new ArgumentException(
                "Title is required.",
                nameof(recurringExpense.Title));
        }

        if (recurringExpense.Amount <= 0)
            throw new ArgumentException(
                "Amount must be greater than zero.",
                nameof(recurringExpense.Amount));

        if (recurringExpense.DayOfMonth < 1 ||
            recurringExpense.DayOfMonth > 31)
        {
            throw new ArgumentException(
                "Day of month must be between 1 and 31.",
                nameof(recurringExpense.DayOfMonth));
        }

        recurringExpense.UserId = userId;
        recurringExpense.Title =
            recurringExpense.Title.Trim();

        await _repository.AddAsync(
            recurringExpense);
    }

    public async Task<List<RecurringExpense>> GetAllAsync(
        string userId)
    {
        return await _repository.GetAllAsync(
            userId);
    }

    public async Task<List<RecurringExpense>> GetActiveAsync(
        string userId)
    {
        return await _repository.GetActiveAsync(
            userId);
    }

    public async Task<List<RecurringExpense>>
        GetActiveForBackgroundAsync()
    {
        return await _repository.GetActiveForBackgroundAsync();
    }

    public async Task<RecurringExpense?> GetByIdAsync(
        int id,
        string userId)
    {
        if (id <= 0)
            return null;

        return await _repository.GetByIdAsync(
            id,
            userId);
    }

    public async Task UpdateAsync(
        RecurringExpense recurringExpense,
        string userId)
    {
        if (recurringExpense == null)
            throw new ArgumentNullException(
                nameof(recurringExpense));

        if (string.IsNullOrWhiteSpace(userId))
            return;

        if (recurringExpense.Id <= 0)
            return;

        if (string.IsNullOrWhiteSpace(
                recurringExpense.Title))
        {
            throw new ArgumentException(
                "Title is required.",
                nameof(recurringExpense.Title));
        }

        if (recurringExpense.Amount <= 0)
            throw new ArgumentException(
                "Amount must be greater than zero.",
                nameof(recurringExpense.Amount));

        if (recurringExpense.DayOfMonth < 1 ||
            recurringExpense.DayOfMonth > 31)
        {
            throw new ArgumentException(
                "Day of month must be between 1 and 31.",
                nameof(recurringExpense.DayOfMonth));
        }

        var existing =
            await _repository.GetByIdAsync(
                recurringExpense.Id,
                userId);

        if (existing == null)
            return;

        existing.Title =
            recurringExpense.Title.Trim();

        existing.Amount =
            recurringExpense.Amount;

        existing.DayOfMonth =
            recurringExpense.DayOfMonth;

        existing.IsActive =
            recurringExpense.IsActive;

        await _repository.UpdateAsync(
            existing);
    }

    public async Task<bool> GenerateExpenseForUserAsync(
        int id,
        string userId)
    {
        if (id <= 0)
            return false;

        if (string.IsNullOrWhiteSpace(userId))
            return false;

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

    public async Task GenerateExpenseAsync(
        int id)
    {
        if (id <= 0)
            return;

        await _generationLock.WaitAsync();

        try
        {
            var recurringExpense =
                await _repository.GetByIdForBackgroundAsync(
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

    public async Task DeleteAsync(
        int id,
        string userId)
    {
        if (id <= 0)
            return;

        if (string.IsNullOrWhiteSpace(userId))
            return;

        await _repository.DeleteAsync(
            id,
            userId);
    }

    private async Task<bool> GenerateExpenseInternalAsync(
        RecurringExpense recurringExpense)
    {
        if (!recurringExpense.IsActive)
            return false;

        var now = DateTime.UtcNow;

        if (recurringExpense.LastGeneratedDate.HasValue &&
            recurringExpense.LastGeneratedDate.Value.Year ==
                now.Year &&
            recurringExpense.LastGeneratedDate.Value.Month ==
                now.Month)
        {
            return false;
        }

        if (now.Day < recurringExpense.DayOfMonth)
            return false;

        var daysInMonth =
            DateTime.DaysInMonth(
                now.Year,
                now.Month);

        var generationDay =
            Math.Min(
                recurringExpense.DayOfMonth,
                daysInMonth);

        if (now.Day < generationDay)
            return false;

        var expenseDate =
            new DateTime(
                now.Year,
                now.Month,
                generationDay);

        var expense = new Expense
        {
            Title = recurringExpense.Title,
            Amount = recurringExpense.Amount,
            DueDate = expenseDate,
            Category = "Recurring",
            IsPaid = false
        };

        await _expenseService.AddExpenseAsync(
            expense,
            recurringExpense.UserId);

        recurringExpense.LastGeneratedDate =
            now;

        await _repository.UpdateAsync(
            recurringExpense);

        return true;
    }
}