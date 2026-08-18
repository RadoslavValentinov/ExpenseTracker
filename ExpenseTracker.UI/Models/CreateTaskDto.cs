using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.UI.Models;

public class CreateTaskDto
{
    [Required(ErrorMessage = "Title is required.")]
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Required(ErrorMessage = "Due date is required.")]
    public DateTime DueDate { get; set; } = DateTime.Now;

    public int Priority { get; set; }

    public DateTime? ReminderTime { get; set; }
}