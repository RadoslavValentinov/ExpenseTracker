using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.API.DTOs;

public class CreateReminderDto
{
    [Required]
    [StringLength(200, MinimumLength = 1)]
    public string Title { get; set; } = string.Empty;

    public DateTime ReminderDate { get; set; }

    [Required]
    [StringLength(50, MinimumLength = 1)]
    public string Type { get; set; } = string.Empty;

    [Range(0, int.MaxValue)]
    public int ReferenceId { get; set; }
}