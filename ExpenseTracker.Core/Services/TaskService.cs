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
        if (task == null)
            throw new ArgumentNullException(nameof(task));

        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException(
                "User id is required.",
                nameof(userId));

        if (string.IsNullOrWhiteSpace(task.Title))
            throw new ArgumentException(
                "Task title is required.",
                nameof(task.Title));

        if (task.DueDate == DateTime.MinValue)
            throw new ArgumentException(
                "Task due date is required.",
                nameof(task.DueDate));

        if (task.Priority < 0 || task.Priority > 5)
            throw new ArgumentException(
                "Task priority must be between 0 and 5.",
                nameof(task.Priority));

        task.UserId = userId;
        task.Title = task.Title.Trim();

        if (!string.IsNullOrWhiteSpace(task.Description))
            task.Description = task.Description.Trim();
        else
            task.Description = null;

        task.IsCompleted = false;
        task.HasReminder = task.ReminderTime.HasValue;

        await _repository.AddAsync(task);

        if (task.ReminderTime.HasValue)
        {
            var reminder = new Reminder
            {
                Title = $"Task: {task.Title}",
                ReminderDate = task.ReminderTime.Value,
                Type = "Task",
                ReferenceId = task.Id,
                IsTriggered = false,
                IsRead = false
            };

            await _reminderService.AddAsync(
                reminder,
                userId);
        }
    }

    public async Task<List<TaskItem>> GetTasksAsync(
        string userId)
    {
        return await _repository.GetAllAsync(
            userId);
    }

    public async Task<bool> CompleteTaskAsync(
        int id,
        string userId)
    {
        var task =
            await _repository.GetByIdAsync(
                id,
                userId);

        if (task == null)
            return false;

        if (task.IsCompleted)
            return true;

        task.IsCompleted = true;

        await _repository.UpdateAsync(
            task);

        return true;
    }

    public async Task<bool> UpdateAsync(
        TaskItem task,
        string userId)
    {
        if (task == null)
            throw new ArgumentNullException(nameof(task));

        if (string.IsNullOrWhiteSpace(userId))
            return false;

        if (task.Id <= 0)
            return false;

        if (string.IsNullOrWhiteSpace(task.Title))
            throw new ArgumentException(
                "Task title is required.",
                nameof(task.Title));

        if (task.DueDate == DateTime.MinValue)
            throw new ArgumentException(
                "Task due date is required.",
                nameof(task.DueDate));

        if (task.Priority < 0 || task.Priority > 5)
            throw new ArgumentException(
                "Task priority must be between 0 and 5.",
                nameof(task.Priority));

        var existingTask =
            await _repository.GetByIdAsync(
                task.Id,
                userId);

        if (existingTask == null)
            return false;

        var oldReminderTime =
            existingTask.ReminderTime;

        existingTask.Title =
            task.Title.Trim();

        existingTask.Description =
            string.IsNullOrWhiteSpace(task.Description)
                ? null
                : task.Description.Trim();

        existingTask.DueDate =
            task.DueDate;

        existingTask.Priority =
            task.Priority;

        existingTask.ReminderTime =
            task.ReminderTime;

        existingTask.HasReminder =
            task.ReminderTime.HasValue;

        existingTask.RepeatMonthly =
            task.RepeatMonthly;

        // IsCompleted is intentionally NOT changed here.
        // Completion is handled only by CompleteTaskAsync.

        await _repository.UpdateAsync(
            existingTask);

        var existingReminder =
            await _reminderService.GetByReferenceAsync(
                "Task",
                existingTask.Id,
                userId);

        if (existingTask.ReminderTime.HasValue)
        {
            if (existingReminder == null)
            {
                var newReminder = new Reminder
                {
                    Title = $"Task: {existingTask.Title}",
                    ReminderDate =
                        existingTask.ReminderTime.Value,
                    Type = "Task",
                    ReferenceId = existingTask.Id,
                    IsTriggered = false,
                    IsRead = false
                };

                await _reminderService.AddAsync(
                    newReminder,
                    userId);
            }
            else
            {
                existingReminder.Title =
                    $"Task: {existingTask.Title}";

                existingReminder.ReminderDate =
                    existingTask.ReminderTime.Value;

                if (oldReminderTime !=
                    existingTask.ReminderTime)
                {
                    existingReminder.IsTriggered = false;
                    existingReminder.IsRead = false;
                }

                await _reminderService.UpdateAsync(
                    existingReminder,
                    userId);
            }
        }
        else
        {
            if (existingReminder != null)
            {
                await _reminderService.DeleteByReferenceAsync(
                    "Task",
                    existingTask.Id,
                    userId);
            }
        }

        return true;
    }

    public async Task<bool> DeleteAsync(
        int id,
        string userId)
    {
        var task =
            await _repository.GetByIdAsync(
                id,
                userId);

        if (task == null)
            return false;

        await _reminderService.DeleteByReferenceAsync(
            "Task",
            id,
            userId);

        await _repository.DeleteAsync(
            id,
            userId);

        return true;
    }
}