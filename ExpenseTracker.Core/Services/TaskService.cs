using ExpenseTracker.Core.Interfaces;
using ExpenseTracker.Core.Models;

namespace ExpenseTracker.Core.Services;

public class TaskService : ITaskService
{
    private readonly ITaskRepository _repository;
    private readonly IReminderService _reminderService;

    public TaskService(
        ITaskRepository repository,
        IReminderService reminderService)
    {
        _repository = repository;
        _reminderService = reminderService;
    }

    public async Task AddTaskAsync(TaskItem task)
    {
        await _repository.AddAsync(task);

        if (task.DueDate.HasValue)
        {
            var reminder = new Reminder
            {
                Title = $"Task: {task.Title}",
                ReminderDate = task.DueDate.Value,
                Type = "Task",
                ReferenceId = task.Id
            };

            await _reminderService.AddAsync(reminder);
        }
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