using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.API.DTOs;

public class UpdateRecurringExpenseDto
{
    [Required]
    [StringLength(200, MinimumLength = 1)]
    public string Title { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    [Range(1, 31)]
    public int DayOfMonth { get; set; }

    public bool IsActive { get; set; }
}