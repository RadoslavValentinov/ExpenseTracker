using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.API.DTOs;

public class CreateRecurringExpenseDto
{
    [Required]
    [MaxLength(100)]
    public string Title { get; set; } = null!;

    [Range(0.01, 100000)]
    public decimal Amount { get; set; }

    [Range(1, 31)]
    public int DayOfMonth { get; set; }

    public bool IsActive { get; set; } = true;
}