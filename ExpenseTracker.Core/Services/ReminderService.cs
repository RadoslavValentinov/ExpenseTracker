using ExpenseTracker.Core.Interfaces;
using ExpenseTracker.Core.Models;

namespace ExpenseTracker.Core.Services;

public class ReminderService : IReminderService
{
    private readonly IReminderRepository _repository;

    public ReminderService(IReminderRepository repository)
    {
        _repository = repository;
    }

    public async Task AddAsync(Reminder reminder)
    {
        await _repository.AddAsync(reminder);
    }

    public async Task<List<Reminder>> GetCompletedAsync()
    {
        return await _repository.GetCompletedAsync();
    }

    public async Task<List<Reminder>> GetPendingAsync()
    {
        return await _repository.GetPendingAsync();
       
    }

    public async Task UpdateAsync(Reminder reminder)
    {
        await _repository.UpdateAsync(reminder);
    }

    public async Task MarkAsTriggeredAsync(int id)
    {
        await _repository.MarkAsTriggeredAsync(id);
    }

    public async Task MarkAsReadAsync(int id)
    {
        await _repository.MarkAsReadAsync(id);
    }

    public async Task DeleteAsync(int id)
    {
        await _repository.DeleteAsync(id);
    }
}