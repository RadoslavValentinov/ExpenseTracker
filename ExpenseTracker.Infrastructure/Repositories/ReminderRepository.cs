using ExpenseTracker.Core.Interfaces;
using ExpenseTracker.Core.Models;
using ExpenseTracker.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Infrastructure.Repositories;

public class ReminderRepository : IReminderRepository
{
    private readonly AppDbContext _context;

    public ReminderRepository(AppDbContext context)
    {
        _context = context;
    }


    public async Task AddAsync(Reminder reminder)
    {
        await _context.Reminders.AddAsync(reminder);

        await _context.SaveChangesAsync();
    }


    public async Task<List<Reminder>> GetPendingAsync(
        string userId)
    {
        return await _context.Reminders
            .Where(r =>
                r.UserId == userId &&
                !r.IsTriggered)
            .OrderBy(r => r.ReminderDate)
            .ToListAsync();
    }


    public async Task<List<Reminder>> GetCompletedAsync(
        string userId)
    {
        return await _context.Reminders
            .Where(r =>
                r.UserId == userId &&
                r.IsTriggered)
            .OrderByDescending(r => r.ReminderDate)
            .ToListAsync();
    }


    public async Task<Reminder?> GetByIdAsync(
        int id,
        string userId)
    {
        return await _context.Reminders
            .FirstOrDefaultAsync(r =>
                r.Id == id &&
                r.UserId == userId);
    }


    public async Task UpdateAsync(Reminder reminder)
    {
        _context.Reminders.Update(reminder);

        await _context.SaveChangesAsync();
    }


    public async Task MarkAsTriggeredAsync(
        int id,
        string userId)
    {
        var reminder =
            await _context.Reminders
                .FirstOrDefaultAsync(r =>
                    r.Id == id &&
                    r.UserId == userId);

        if (reminder == null)
            return;

        reminder.IsTriggered = true;

        await _context.SaveChangesAsync();
    }


    public async Task MarkAsReadAsync(
        int id,
        string userId)
    {
        var reminder =
            await _context.Reminders
                .FirstOrDefaultAsync(r =>
                    r.Id == id &&
                    r.UserId == userId);

        if (reminder == null)
            return;

        reminder.IsRead = true;

        await _context.SaveChangesAsync();
    }


    public async Task DeleteAsync(
        int id,
        string userId)
    {
        var reminder =
            await _context.Reminders
                .FirstOrDefaultAsync(r =>
                    r.Id == id &&
                    r.UserId == userId);

        if (reminder == null)
            return;

        _context.Reminders.Remove(reminder);

        await _context.SaveChangesAsync();
    }


    // =========================
    // BACKGROUND PROCESSING
    // =========================

    public async Task<List<Reminder>> GetPendingForBackgroundAsync()
    {
        return await _context.Reminders
            .Where(r => !r.IsTriggered)
            .OrderBy(r => r.ReminderDate)
            .ToListAsync();
    }


    public async Task MarkAsTriggeredForBackgroundAsync(int id)
    {
        var reminder =
            await _context.Reminders
                .FirstOrDefaultAsync(r => r.Id == id);

        if (reminder == null)
            return;

        reminder.IsTriggered = true;

        await _context.SaveChangesAsync();
    }
}