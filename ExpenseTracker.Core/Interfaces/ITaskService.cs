using ExpenseTracker.Core.Models;

namespace ExpenseTracker.Core.Interfaces;

public interface ITaskService
{
    Task AddTaskAsync(
        TaskItem task,
        string userId);

    Task<List<TaskItem>> GetTasksAsync(
        string userId);

    Task CompleteTaskAsync(
        int id,
        string userId);

    Task UpdateAsync(
        TaskItem task,
        string userId);

    Task DeleteAsync(
        int id,
        string userId);
}