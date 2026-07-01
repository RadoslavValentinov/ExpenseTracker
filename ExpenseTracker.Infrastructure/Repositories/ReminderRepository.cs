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

    public async Task<List<Reminder>> GetAllAsync()
    {
        return await _context.Reminders.ToListAsync();
    }

    public async Task<List<Reminder>> GetCompletedAsync()
    {
        return await _context.Reminders
            .Where(r => r.IsTriggered)
            .OrderByDescending(r => r.ReminderDate)
            .ToListAsync();
    }

    public async Task UpdateAsync(Reminder reminder)
    {
        _context.Reminders.Update(reminder);

        await _context.SaveChangesAsync();
    }

    public async Task<List<Reminder>> GetPendingAsync()
    {
        return await _context.Reminders
            .Where(r => !r.IsTriggered)
            .OrderBy(r => r.ReminderDate)
            .ToListAsync();
    }

    public async Task MarkAsTriggeredAsync(int id)
    {
        var reminder = await _context.Reminders.FindAsync(id);

        if (reminder == null)
            return;

        reminder.IsTriggered = true;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var reminder = await _context.Reminders.FindAsync(id);

        if (reminder != null)
        {
            _context.Reminders.Remove(reminder);
            await _context.SaveChangesAsync();
        }
    }
}