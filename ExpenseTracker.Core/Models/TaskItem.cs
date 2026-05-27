public class TaskItem
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public DateTime DueDate { get; set; }

    public int Priority { get; set; }

    public bool IsCompleted { get; set; }

    public bool HasReminder { get; set; }

    public DateTime? ReminderTime { get; set; }

    public bool RepeatMonthly { get; set; }

    public DateTime CreatedDate { get; set; }
        = DateTime.Now;
}