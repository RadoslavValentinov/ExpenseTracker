using ExpenseTracker.Core.Models;

namespace ExpenseTracker.Core.Interfaces;

public interface IReminderService
{
    Task AddAsync(Reminder reminder);
    Task<List<Reminder>> GetPendingAsync();
    Task<List<Reminder>> GetCompletedAsync();
    Task MarkAsTriggeredAsync(int id);
}