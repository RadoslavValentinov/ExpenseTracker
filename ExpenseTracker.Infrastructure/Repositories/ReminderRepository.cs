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

    public Task<List<Reminder>> GetPendingAsync()
    {
        throw new NotImplementedException();
    }

    public Task MarkAsTriggeredAsync(int id)
    {
        throw new NotImplementedException();
    }
}