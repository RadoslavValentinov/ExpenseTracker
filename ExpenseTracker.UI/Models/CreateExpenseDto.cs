namespace ExpenseTracker.UI.Models;

public class CreateExpenseDto
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public DateTime DueDate { get; set; } = DateTime.Now;

    public string? Category { get; set; }
}