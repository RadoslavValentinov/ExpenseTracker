using ExpenseTracker.Core.Interfaces;
using ExpenseTracker.Core.Models;

namespace ExpenseTracker.Core.Services;

public class ReminderService : IReminderService
{
    private readonly IReminderRepository _repository;

    public ReminderService(
        IReminderRepository repository)
    {
        _repository = repository;
    }


    public async Task AddAsync(
        Reminder reminder,
        string userId)
    {
        reminder.UserId = userId;

        await _repository.AddAsync(reminder);
    }


    public async Task<List<Reminder>> GetPendingAsync(
        string userId)
    {
        return await _repository.GetPendingAsync(
            userId);
    }


    public async Task<List<Reminder>> GetCompletedAsync(
        string userId)
    {
        return await _repository.GetCompletedAsync(
            userId);
    }


    public async Task<Reminder?> GetByIdAsync(
        int id,
        string userId)
    {
        return await _repository.GetByIdAsync(
            id,
            userId);
    }


    public async Task MarkAsTriggeredAsync(
        int id,
        string userId)
    {
        await _repository.MarkAsTriggeredAsync(
            id,
            userId);
    }


    public async Task MarkAsReadAsync(
        int id,
        string userId)
    {
        await _repository.MarkAsReadAsync(
            id,
            userId);
    }


    public async Task DeleteAsync(
        int id,
        string userId)
    {
        await _repository.DeleteAsync(
            id,
            userId);
    }


    public async Task UpdateAsync(
        Reminder reminder,
        string userId)
    {
        var existing =
            await _repository.GetByIdAsync(
                reminder.Id,
                userId);

        if (existing == null)
            return;

        existing.Title =
            reminder.Title;

        existing.ReminderDate =
            reminder.ReminderDate;

        existing.Type =
            reminder.Type;

        existing.ReferenceId =
            reminder.ReferenceId;

        existing.IsTriggered =
            reminder.IsTriggered;

        existing.IsRead =
            reminder.IsRead;

        await _repository.UpdateAsync(
            existing);
    }


    // =========================
    // BACKGROUND PROCESSING
    // =========================

    public Task<List<Reminder>> GetPendingForBackgroundAsync()
    {
        return _repository.GetPendingForBackgroundAsync();
    }


    public Task MarkAsTriggeredForBackgroundAsync(int id)
    {
        return _repository.MarkAsTriggeredForBackgroundAsync(id);
    }
}