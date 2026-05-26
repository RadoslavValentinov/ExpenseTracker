using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.API.DTOs;

public class CreateTaskDto
{
    [Required]
    [MaxLength(100)]
    public string Title { get; set; } = null!;

    [MaxLength(500)]
    public string? Description { get; set; }

    public DateTime? DueDate { get; set; }

    [Range(1, 5)]
    public int Priority { get; set; }
}