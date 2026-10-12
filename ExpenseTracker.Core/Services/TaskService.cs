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
        task.HasReminder = task.ReminderTime.HasValue;

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

        task.IsCompleted = true;

        await _repository.UpdateAsync(
            task);

        return true;
    }


    public async Task<bool> UpdateAsync(
        TaskItem task,
        string userId)
    {
        var existingTask =
            await _repository.GetByIdAsync(
                task.Id,
                userId);

        if (existingTask == null)
            return false;


        var oldReminderTime =
            existingTask.ReminderTime;


        existingTask.Title =
            task.Title;

        existingTask.Description =
            task.Description;

        existingTask.DueDate =
            task.DueDate;

        existingTask.Priority =
            task.Priority;

        existingTask.ReminderTime =
            task.ReminderTime;

        existingTask.IsCompleted =
            task.IsCompleted;

        existingTask.HasReminder =
            task.ReminderTime.HasValue;

        existingTask.RepeatMonthly =
            task.RepeatMonthly;


        await _repository.UpdateAsync(
            existingTask);


        var existingReminder =
            await _reminderService.GetByReferenceAsync(
                "Task",
                existingTask.Id,
                userId);


        // =========================
        // TASK HAS REMINDER
        // =========================

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
                    ReferenceId = existingTask.Id
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


                // If the reminder time changed,
                // make the reminder active again.
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

        // =========================
        // TASK NO LONGER HAS REMINDER
        // =========================

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


        // Delete the related reminder first.
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