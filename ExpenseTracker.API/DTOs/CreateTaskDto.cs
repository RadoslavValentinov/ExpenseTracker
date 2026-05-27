namespace ExpenseTracker.API.DTOs;

public class CreateTaskDto
{
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public DateTime DueDate { get; set; }

    public int Priority { get; set; }

    public DateTime? ReminderTime { get; set; }
}