using ExpenseTracker.Core.Interfaces;
using ExpenseTracker.Core.Models;

namespace ExpenseTracker.Core.Services;

public class TaskService : ITaskService
{
    private readonly ITaskRepository _repository;

    public TaskService(ITaskRepository repository)
    {
        _repository = repository;
    }

    public async Task AddTaskAsync(TaskItem task)
    {
        await _repository.AddAsync(task);
    }

    public async Task<List<TaskItem>> GetTasksAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task CompleteTaskAsync(int id)
    {
        var task = await _repository.GetByIdAsync(id);
        if (task == null) return;

        task.IsCompleted = true;
        await _repository.UpdateAsync(task);
    }

    public async Task DeleteTaskAsync(int id)
    {
        await _repository.DeleteAsync(id);
    }
}