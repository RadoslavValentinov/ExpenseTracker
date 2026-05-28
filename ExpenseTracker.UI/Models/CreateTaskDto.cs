namespace ExpenseTracker.UI.Models;

public class CreateTaskDto
{
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public DateTime DueDate { get; set; }
        = DateTime.Now;

    public int Priority { get; set; }

    public DateTime? ReminderTime { get; set; }
}