using ExpenseTracker.Core.Models;

namespace ExpenseTracker.Core.Interfaces;

public interface ITaskService
{
    Task AddTaskAsync(TaskItem task);

    Task<List<TaskItem>> GetTasksAsync();

    Task CompleteTaskAsync(int id);

    Task DeleteAsync(int id);
}