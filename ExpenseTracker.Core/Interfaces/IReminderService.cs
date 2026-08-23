using ExpenseTracker.Core.Models;

namespace ExpenseTracker.Core.Interfaces;

public interface IReminderService
{
    Task AddAsync(
        Reminder reminder,
        string userId);

    Task<List<Reminder>> GetPendingAsync(
        string userId);

    Task<List<Reminder>> GetCompletedAsync(
        string userId);

    Task<Reminder?> GetByIdAsync(
        int id,
        string userId);

    Task MarkAsTriggeredAsync(
        int id,
        string userId);

    Task DeleteAsync(
        int id,
        string userId);

    Task UpdateAsync(
        Reminder reminder,
        string userId);

    Task MarkAsReadAsync(
        int id,
        string userId);

    // Used only by background processing
    Task<List<Reminder>> GetPendingForBackgroundAsync();

    Task MarkAsTriggeredForBackgroundAsync(int id);
}