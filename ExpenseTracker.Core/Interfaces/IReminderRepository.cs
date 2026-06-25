using ExpenseTracker.Core.Models;

namespace ExpenseTracker.Core.Interfaces;

public interface IReminderRepository
{
    Task AddAsync(Reminder reminder);
    Task<List<Reminder>> GetCompletedAsync();
    Task<List<Reminder>> GetAllAsync();
    Task<List<Reminder>> GetPendingAsync();
    Task MarkAsTriggeredAsync(int id);
    Task DeleteAsync(int id);
}