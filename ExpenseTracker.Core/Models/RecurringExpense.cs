namespace ExpenseTracker.Core.Models;

public class RecurringExpense
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public decimal Amount { get; set; }

    public int DayOfMonth { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? LastGeneratedDate { get; set; }
}