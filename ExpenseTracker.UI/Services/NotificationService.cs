using ExpenseTracker.Core.Models;
using ExpenseTracker.UI.Models;

namespace ExpenseTracker.UI.Services;

public class NotificationService
{
    private readonly ExpenseApiService _expenseService;
    private readonly TaskApiService _taskService;
    private readonly ReminderApiService _reminderService;

    public NotificationService(
        ExpenseApiService expenseService,
        TaskApiService taskService,
        ReminderApiService reminderService)
    {
        _expenseService = expenseService;
        _taskService = taskService;
        _reminderService = reminderService;
    }


    public async Task<List<NotificationItem>> GetNotificationsAsync()
    {
        var notifications = new List<NotificationItem>();

        
        var reminders = await _reminderService.GetAllAsync();

        foreach (var reminder in reminders)
        {
            notifications.Add(new NotificationItem
            {
                Id = reminder.Id,
                Title = reminder.Title,
                Description = reminder.Type,
                Type = "Reminder",
                Date = reminder.ReminderDate,
                Url = $"/reminders/{reminder.Id}",
                IsRead = false
            });
        }

        
        var expenses = await _expenseService.GetExpensesAsync();

        foreach (var expense in expenses.Where(x => !x.IsPaid))
        {
            notifications.Add(new NotificationItem
            {
                Id = expense.Id,
                Title = expense.Title,
                Description = $"Due: {expense.DueDate:d}",
                Type = "Expense",
                Date = expense.DueDate,
                Url = $"/expenses/{expense.Id}",
                IsRead = false
            });
        }


        var tasks = await _taskService.GetTasksAsync() ?? new List<TaskItem>();


        foreach (var task in tasks!)
        {
            notifications.Add(new NotificationItem
            {
                Id = task.Id,
                Title = task.Title,
                Description = $"Due: {task.DueDate:d}",
                Type = "Task",
                Date = task.DueDate,
                Url = $"/tasks/{task.Id}",
                IsRead = false
            });
        }

        return notifications
            .OrderBy(x => x.Date)
            .ToList();
    }




}