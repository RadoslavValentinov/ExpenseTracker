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


    public async Task AddTaskAsync(
        TaskItem task,
        string userId)
    {
        task.UserId = userId;

        await _repository.AddAsync(task);

        if (task.ReminderTime.HasValue)
        {
            var reminder = new Reminder
            {
                Title = $"Task: {task.Title}",
                ReminderDate = task.ReminderTime.Value,
                Type = "Task",
                ReferenceId = task.Id
            };

            await _reminderService.AddAsync(reminder);
        }
    }


    public async Task<List<TaskItem>> GetTasksAsync(
        string userId)
    {
        return await _repository.GetAllAsync(userId);
    }


    public async Task CompleteTaskAsync(
        int id,
        string userId)
    {
        var task =
            await _repository.GetByIdAsync(
                id,
                userId);

        if (task == null)
            return;

        task.IsCompleted = true;

        await _repository.UpdateAsync(task);
    }


    public async Task UpdateAsync(
        TaskItem task,
        string userId)
    {
        var existingTask =
            await _repository.GetByIdAsync(
                task.Id,
                userId);

        if (existingTask == null)
            return;

        existingTask.Title = task.Title;
        existingTask.Description = task.Description;
        existingTask.DueDate = task.DueDate;
        existingTask.Priority = task.Priority;
        existingTask.ReminderTime = task.ReminderTime;
        existingTask.IsCompleted = task.IsCompleted;
        existingTask.HasReminder = task.HasReminder;
        existingTask.RepeatMonthly = task.RepeatMonthly;

        await _repository.UpdateAsync(existingTask);
    }


    public async Task DeleteAsync(
        int id,
        string userId)
    {
        await _repository.DeleteAsync(
            id,
            userId);
    }
}