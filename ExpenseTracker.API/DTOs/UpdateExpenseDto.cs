using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.API.DTOs;

public class UpdateExpenseDto
{
    [Required]
    [StringLength(200, MinimumLength = 1)]
    public string Title { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public DateTime DueDate { get; set; }

    [StringLength(100)]
    public string? Category { get; set; }
}