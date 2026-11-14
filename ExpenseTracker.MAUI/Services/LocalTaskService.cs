using ExpenseTracker.Core.Models;
using ExpenseTracker.MAUI.Data;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.MAUI.Services;

public class LocalTaskService
{
    private readonly IDbContextFactory<ExpenseDbContext> _dbFactory;

    public LocalTaskService(IDbContextFactory<ExpenseDbContext> dbFactory)
    {
        _dbFactory = dbFactory;
    }

    public async Task<List<TaskItem>> GetTasksAsync()
    {
        await using var db = await _dbFactory.CreateDbContextAsync();

        return await db.Tasks
            .OrderBy(t => t.IsCompleted)
            .ThenBy(t => t.DueDate)
            .ThenByDescending(t => t.Priority)
            .ThenByDescending(t => t.Id)
            .ToListAsync();
    }

    public async Task<TaskItem> AddTaskAsync(
    string title,
    string? description,
    DateTime dueDate,
    int priority,
    bool hasReminder,
    DateTime? reminderTime,
    bool repeatMonthly)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Заглавието е задължително.");

        if (priority < 1 || priority > 3)
            throw new ArgumentException("Приоритетът трябва да бъде между 1 и 3.");

        if (hasReminder && !reminderTime.HasValue)
            throw new ArgumentException(
                "При включено напомняне трябва да бъде зададен час.");

        await using var db = await _dbFactory.CreateDbContextAsync();

        var task = new TaskItem
        {
            UserId = "local-user",
            Title = title.Trim(),
            Description = string.IsNullOrWhiteSpace(description)
                ? null
                : description.Trim(),
            DueDate = dueDate,
            Priority = priority,
            IsCompleted = false,
            HasReminder = hasReminder,
            ReminderTime = hasReminder
                ? reminderTime
                : null,
            RepeatMonthly = repeatMonthly,
            CreatedDate = DateTime.Now
        };

        db.Tasks.Add(task);

        await db.SaveChangesAsync();

        return task;
    }

    public async Task UpdateTaskAsync(
    int taskId,
    string title,
    string? description,
    DateTime dueDate,
    int priority,
    bool hasReminder,
    DateTime? reminderTime,
    bool repeatMonthly)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Заглавието е задължително.");

        if (priority < 1 || priority > 3)
            throw new ArgumentException("Приоритетът трябва да бъде между 1 и 3.");

        if (hasReminder && !reminderTime.HasValue)
            throw new ArgumentException(
                "При включено напомняне трябва да бъде зададен час.");

        await using var db = await _dbFactory.CreateDbContextAsync();

        var task = await db.Tasks
            .FirstOrDefaultAsync(t => t.Id == taskId);

        if (task is null)
            throw new InvalidOperationException("Задачата не е намерена.");

        task.Title = title.Trim();

        task.Description = string.IsNullOrWhiteSpace(description)
            ? null
            : description.Trim();

        task.DueDate = dueDate;
        task.Priority = priority;

        task.HasReminder = hasReminder;

        task.ReminderTime = hasReminder
            ? reminderTime
            : null;

        task.RepeatMonthly = repeatMonthly;

        await db.SaveChangesAsync();
    }

    public async Task MarkAsCompletedAsync(int taskId)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();

        var task = await db.Tasks
            .FirstOrDefaultAsync(t => t.Id == taskId);

        if (task is null)
            return;

        task.IsCompleted = true;

        await db.SaveChangesAsync();
    }

    public async Task MarkAsIncompleteAsync(int taskId)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();

        var task = await db.Tasks
            .FirstOrDefaultAsync(t => t.Id == taskId);

        if (task is null)
            return;

        task.IsCompleted = false;

        await db.SaveChangesAsync();
    }

    public async Task DeleteTaskAsync(int taskId)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();

        var task = await db.Tasks
            .FirstOrDefaultAsync(t => t.Id == taskId);

        if (task is null)
            return;

        db.Tasks.Remove(task);

        await db.SaveChangesAsync();
    }
}