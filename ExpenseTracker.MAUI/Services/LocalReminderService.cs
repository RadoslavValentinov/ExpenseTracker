using ExpenseTracker.Core.Models;
using ExpenseTracker.MAUI.Data;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.MAUI.Services;

public class LocalReminderService
{
    private readonly IDbContextFactory<ExpenseDbContext> _dbFactory;

    public LocalReminderService(IDbContextFactory<ExpenseDbContext> dbFactory)
    {
        _dbFactory = dbFactory;
    }

    public async Task<List<Reminder>> GetRemindersAsync()
    {
        await using var db = await _dbFactory.CreateDbContextAsync();

        return await db.Reminders
            .OrderBy(r => r.IsRead)
            .ThenBy(r => r.ReminderDate)
            .ThenByDescending(r => r.Id)
            .ToListAsync();
    }

    public async Task<Reminder> AddReminderAsync(
        string title,
        DateTime reminderDate,
        string type,
        int referenceId)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Заглавието е задължително.");

        if (string.IsNullOrWhiteSpace(type))
            throw new ArgumentException("Типът на напомнянето е задължителен.");

        await using var db = await _dbFactory.CreateDbContextAsync();

        var reminder = new Reminder
        {
            UserId = "local-user",
            Title = title.Trim(),
            ReminderDate = reminderDate,
            IsTriggered = false,
            Type = type.Trim(),
            ReferenceId = referenceId,
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };

        db.Reminders.Add(reminder);

        await db.SaveChangesAsync();

        return reminder;
    }

    public async Task UpdateReminderAsync(
        int reminderId,
        string title,
        DateTime reminderDate)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Заглавието е задължително.");

        await using var db = await _dbFactory.CreateDbContextAsync();

        var reminder = await db.Reminders
            .FirstOrDefaultAsync(r => r.Id == reminderId);

        if (reminder is null)
            throw new InvalidOperationException(
                "Напомнянето не е намерено.");

        reminder.Title = title.Trim();
        reminder.ReminderDate = reminderDate;

        await db.SaveChangesAsync();
    }

    public async Task MarkAsReadAsync(int reminderId)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();

        var reminder = await db.Reminders
            .FirstOrDefaultAsync(r => r.Id == reminderId);

        if (reminder is null)
            return;

        reminder.IsRead = true;

        await db.SaveChangesAsync();
    }

    public async Task MarkAsUnreadAsync(int reminderId)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();

        var reminder = await db.Reminders
            .FirstOrDefaultAsync(r => r.Id == reminderId);

        if (reminder is null)
            return;

        reminder.IsRead = false;

        await db.SaveChangesAsync();
    }

    public async Task DeleteReminderAsync(int reminderId)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();

        var reminder = await db.Reminders
            .FirstOrDefaultAsync(r => r.Id == reminderId);

        if (reminder is null)
            return;

        db.Reminders.Remove(reminder);

        await db.SaveChangesAsync();
    }
}