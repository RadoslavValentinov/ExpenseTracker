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
        expense.UserId = userId;

        await _repository.AddAsync(expense);

        if (expense.DueDate > DateTime.MinValue)
        {
            var reminder = new Reminder
            {
                Title = $"Pay {expense.Title}",
                ReminderDate = expense.DueDate.AddDays(-1),
                Type = "Expense",
                ReferenceId = expense.Id
            };

            await _reminderService.AddAsync(
                reminder,
                userId);
        }
    }


    public async Task<List<Expense>> GetExpensesAsync(
        string userId)
    {
        return await _repository.GetAllAsync(userId);
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

        await _repository.DeleteAsync(
            id,
            userId);

        return true;
    }


    public async Task<bool> UpdateAsync(
        Expense expense,
        string userId)
    {
        var existingExpense =
            await _repository.GetByIdAsync(
                expense.Id,
                userId);

        if (existingExpense == null)
            return false;

        existingExpense.Title =
            expense.Title;

        existingExpense.Amount =
            expense.Amount;

        existingExpense.DueDate =
            expense.DueDate;

        existingExpense.Category =
            expense.Category;

        existingExpense.IsPaid =
            expense.IsPaid;

        await _repository.UpdateAsync(
            existingExpense);

        return true;
    }


    public async Task<List<Expense>> GetByMonthAsync(
        int month,
        string userId)
    {
        return await _repository.GetByMonthAsync(
            month,
            userId);
    }
}