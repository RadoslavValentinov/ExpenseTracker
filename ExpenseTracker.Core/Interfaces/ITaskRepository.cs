using ExpenseTracker.Core.Models;

namespace ExpenseTracker.Core.Interfaces;

public interface ITaskRepository
{
    Task AddAsync(TaskItem task);
    Task<List<TaskItem>> GetAllAsync();
    Task<TaskItem?> GetByIdAsync(int id);
    Task UpdateAsync(TaskItem task);
    Task DeleteAsync(int id);
}