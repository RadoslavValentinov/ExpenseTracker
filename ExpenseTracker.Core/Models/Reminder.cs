namespace ExpenseTracker.Core.Models;

public class Reminder
{
    public int Id { get; set; }

    public string UserId { get; set; } = string.Empty;

    public string Title { get; set; } = null!;

    public DateTime ReminderDate { get; set; }

    public bool IsTriggered { get; set; } = false;

    public string Type { get; set; } = null!;

    public int ReferenceId { get; set; }

    public bool IsRead { get; set; }

    public DateTime CreatedAt { get; set; }
        = DateTime.UtcNow;
}