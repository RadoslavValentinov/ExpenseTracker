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

        await _repository.AddAsync(
            expense);


        var reminder = new Reminder
        {
            Title = $"Expense: {expense.Title}",
            ReminderDate =
                expense.DueDate.AddDays(-1),
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

        expense.IsPaid = true;

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


        // Delete the related reminder first.
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
        var existingExpense =
            await _repository.GetByIdAsync(
                expense.Id,
                userId);

        if (existingExpense == null)
            return false;


        var oldDueDate =
            existingExpense.DueDate;


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


            // If the due date changed,
            // make the reminder active again.
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