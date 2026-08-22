using ExpenseTracker.Core.Models;

namespace ExpenseTracker.Core.Interfaces;

public interface ITaskRepository
{
    Task AddAsync(TaskItem task);

    Task<List<TaskItem>> GetAllAsync(string userId);

    Task<TaskItem?> GetByIdAsync(
        int id,
        string userId);

    Task UpdateAsync(TaskItem task);

    Task DeleteAsync(
        int id,
        string userId);
}