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

    public async Task AddExpenseAsync(Expense expense)
    {
        await _repository.AddAsync(expense);

        var reminder = new Reminder
        {
            Title = $"Pay {expense.Title}",
            ReminderDate = expense.DueDate.AddDays(-1),
            Type = "Expense",
            ReferenceId = expense.Id
        };

        await _reminderService.AddAsync(reminder);
    }

    public async Task<List<Expense>> GetExpensesAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task MarkAsPaidAsync(int id)
    {
        var expense = await _repository.GetByIdAsync(id);
        if (expense == null) return;

        expense.IsPaid = true;
        await _repository.UpdateAsync(expense);
    }

    public async Task DeleteExpenseAsync(int id)
    {
        await _repository.DeleteAsync(id);
    }

    public async Task UpdateExpenseAsync(Expense expense)
    {
        await _repository.UpdateAsync(expense);
    }

    public async Task<List<Expense>> GetByMonthAsync(int month)
    {
        return await _repository.GetByMonthAsync(month);
    }
}