using ExpenseTracker.Core.Interfaces;
using ExpenseTracker.Core.Models;

namespace ExpenseTracker.Core.Services;

public class ExpenseService : IExpenseService
{
    private readonly IExpenseRepository _repository;
    private readonly IReminderService _reminderService;

    public ExpenseService(
        IExpenseRepository repository,
        IReminderService reminderService)
    {
        _repository = repository;
        _reminderService = reminderService;
    }

    public async Task AddExpenseAsync(
        Expense expense,
        string userId)
    {
        if (expense == null)
            throw new ArgumentNullException(nameof(expense));

        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException(
                "User id is required.",
                nameof(userId));

        if (string.IsNullOrWhiteSpace(expense.Title))
            throw new ArgumentException(
                "Expense title is required.",
                nameof(expense.Title));

        if (expense.Amount <= 0)
            throw new ArgumentException(
                "Expense amount must be greater than zero.",
                nameof(expense.Amount));

        if (expense.DueDate == DateTime.MinValue)
            throw new ArgumentException(
                "Expense due date is required.",
                nameof(expense.DueDate));

        expense.UserId = userId;
        expense.Title = expense.Title.Trim();

        if (!string.IsNullOrWhiteSpace(expense.Category))
            expense.Category = expense.Category.Trim();

        expense.IsPaid = false;

        await _repository.AddAsync(
            expense);

        var reminder = new Reminder
        {
            Title = $"Expense: {expense.Title}",
            ReminderDate = expense.DueDate.AddDays(-1),
            Type = "Expense",
            ReferenceId = expense.Id
        };

        await _reminderService.AddAsync(
            reminder,
            userId);
    }

    public async Task<List<Expense>> GetExpensesAsync(
        string userId)
    {
        return await _repository.GetAllAsync(
            userId);
    }

    public async Task<bool> MarkAsPaidAsync(
        int id,
        string userId)
    {
        var expense =
            await _repository.GetByIdAsync(
                id,
                userId);

        if (expense == null)
            return false;

        if (expense.IsPaid)
            return true;

        await _repository.MarkAsPaidAsync(
            id,
            userId);

        return true;
    }

    public async Task<bool> DeleteAsync(
        int id,
        string userId)
    {
        var expense =
            await _repository.GetByIdAsync(
                id,
                userId);

        if (expense == null)
            return false;

        await _reminderService.DeleteByReferenceAsync(
            "Expense",
            id,
            userId);

        await _repository.DeleteAsync(
            id,
            userId);

        return true;
    }

    public async Task<bool> UpdateAsync(
        Expense expense,
        string userId)
    {
        if (expense == null)
            throw new ArgumentNullException(nameof(expense));

        if (string.IsNullOrWhiteSpace(userId))
            return false;

        if (expense.Id <= 0)
            return false;

        if (string.IsNullOrWhiteSpace(expense.Title))
            throw new ArgumentException(
                "Expense title is required.",
                nameof(expense.Title));

        if (expense.Amount <= 0)
            throw new ArgumentException(
                "Expense amount must be greater than zero.",
                nameof(expense.Amount));

        if (expense.DueDate == DateTime.MinValue)
            throw new ArgumentException(
                "Expense due date is required.",
                nameof(expense.DueDate));

        var existingExpense =
            await _repository.GetByIdAsync(
                expense.Id,
                userId);

        if (existingExpense == null)
            return false;

        var oldDueDate =
            existingExpense.DueDate;

        existingExpense.Title =
            expense.Title.Trim();

        existingExpense.Amount =
            expense.Amount;

        existingExpense.DueDate =
            expense.DueDate;

        existingExpense.Category =
            string.IsNullOrWhiteSpace(expense.Category)
                ? null
                : expense.Category.Trim();

        // IsPaid is intentionally NOT changed here.
        // Payment is handled only by MarkAsPaidAsync.

        await _repository.UpdateAsync(
            existingExpense);

        var existingReminder =
            await _reminderService.GetByReferenceAsync(
                "Expense",
                existingExpense.Id,
                userId);

        if (existingReminder != null)
        {
            existingReminder.Title =
                $"Expense: {existingExpense.Title}";

            existingReminder.ReminderDate =
                existingExpense.DueDate.AddDays(-1);

            if (oldDueDate !=
                existingExpense.DueDate)
            {
                existingReminder.IsTriggered = false;
                existingReminder.IsRead = false;
            }

            await _reminderService.UpdateAsync(
                existingReminder,
                userId);
        }
        else
        {
            var reminder = new Reminder
            {
                Title = $"Expense: {existingExpense.Title}",
                ReminderDate =
                    existingExpense.DueDate.AddDays(-1),
                Type = "Expense",
                ReferenceId = existingExpense.Id,
                IsTriggered = false,
                IsRead = false
            };

            await _reminderService.AddAsync(
                reminder,
                userId);
        }

        return true;
    }

    public async Task<List<Expense>> GetByMonthAsync(
        int month,
        string userId)
    {
        if (month < 1 || month > 12)
            return new List<Expense>();

        return await _repository.GetByMonthAsync(
            month,
            userId);
    }
}