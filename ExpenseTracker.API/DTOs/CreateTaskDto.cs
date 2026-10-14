using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.API.DTOs;

public class CreateTaskDto
{
    [Required]
    [StringLength(200, MinimumLength = 1)]
    public string Title { get; set; } = string.Empty;

    [StringLength(2000)]
    public string? Description { get; set; }

    public DateTime DueDate { get; set; }

    [Range(0, 5)]
    public int Priority { get; set; }

    public DateTime? ReminderTime { get; set; }

    public bool RepeatMonthly { get; set; }
}