using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.API.DTOs;

public class UpdateReminderDto
{
    [Required]
    [StringLength(200, MinimumLength = 1)]
    public string Title { get; set; } = string.Empty;

    public DateTime ReminderDate { get; set; }
}