using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.API.DTOs;

public class CreateExpenseDto
{
    [Required]
    [MaxLength(100)]
    public string Title { get; set; } = null!;

    [Range(0.01, 100000)]
    public decimal Amount { get; set; }

    [Required]
    public DateTime DueDate { get; set; }

    [MaxLength(50)]
    public string? Category { get; set; }
}