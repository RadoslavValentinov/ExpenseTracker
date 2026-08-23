using ExpenseTracker.Core.Models;

namespace ExpenseTracker.Core.Interfaces;

public interface ITaskService
{
    Task AddTaskAsync(
        TaskItem task,
        string userId);

    Task<List<TaskItem>> GetTasksAsync(
        string userId);

    Task<bool> CompleteTaskAsync(
        int id,
        string userId);

    Task<bool> UpdateAsync(
        TaskItem task,
        string userId);

    Task<bool> DeleteAsync(
        int id,
        string userId);
}