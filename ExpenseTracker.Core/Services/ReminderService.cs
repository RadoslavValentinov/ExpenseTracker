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
        if (reminder == null)
            throw new ArgumentNullException(nameof(reminder));

        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException(
                "User id is required.",
                nameof(userId));

        if (string.IsNullOrWhiteSpace(reminder.Title))
            throw new ArgumentException(
                "Reminder title is required.",
                nameof(reminder.Title));

        if (reminder.ReminderDate == DateTime.MinValue)
            throw new ArgumentException(
                "Reminder date is required.",
                nameof(reminder.ReminderDate));

        if (string.IsNullOrWhiteSpace(reminder.Type))
            throw new ArgumentException(
                "Reminder type is required.",
                nameof(reminder.Type));

        if (reminder.ReferenceId < 0)
            throw new ArgumentException(
                "Reference id cannot be negative.",
                nameof(reminder.ReferenceId));

        reminder.UserId = userId;
        reminder.Title = reminder.Title.Trim();
        reminder.Type = reminder.Type.Trim();

        // These states are controlled by the server.
        reminder.IsTriggered = false;
        reminder.IsRead = false;

        await _repository.AddAsync(
            reminder);
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

    public async Task<Reminder?> GetByReferenceAsync(
        string type,
        int referenceId,
        string userId)
    {
        if (string.IsNullOrWhiteSpace(type))
            return null;

        if (referenceId <= 0)
            return null;

        return await _repository.GetByReferenceAsync(
            type,
            referenceId,
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

    public async Task DeleteByReferenceAsync(
        string type,
        int referenceId,
        string userId)
    {
        if (string.IsNullOrWhiteSpace(type))
            return;

        if (referenceId <= 0)
            return;

        await _repository.DeleteByReferenceAsync(
            type,
            referenceId,
            userId);
    }

    public async Task UpdateAsync(
        Reminder reminder,
        string userId)
    {
        if (reminder == null)
            throw new ArgumentNullException(nameof(reminder));

        if (string.IsNullOrWhiteSpace(userId))
            return;

        if (reminder.Id <= 0)
            return;

        if (string.IsNullOrWhiteSpace(reminder.Title))
            throw new ArgumentException(
                "Reminder title is required.",
                nameof(reminder.Title));

        if (reminder.ReminderDate == DateTime.MinValue)
            throw new ArgumentException(
                "Reminder date is required.",
                nameof(reminder.ReminderDate));

        var existing =
            await _repository.GetByIdAsync(
                reminder.Id,
                userId);

        if (existing == null)
            return;

        // Only these two fields are editable
        // through the general update endpoint.
        existing.Title =
            reminder.Title.Trim();

        existing.ReminderDate =
            reminder.ReminderDate;

        // UserId stays unchanged.
        // Type stays unchanged.
        // ReferenceId stays unchanged.
        // IsTriggered stays unchanged.
        // IsRead stays unchanged.

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

    public Task MarkAsTriggeredForBackgroundAsync(
        int id)
    {
        return _repository.MarkAsTriggeredForBackgroundAsync(
            id);
    }
}