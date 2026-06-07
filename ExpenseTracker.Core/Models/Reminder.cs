namespace ExpenseTracker.Core.Models;

public class Reminder
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public DateTime ReminderDate { get; set; }

    public bool IsTriggered { get; set; } = false;// problem s towa vsichki zapisi w bazata sa true a trqbva da gi zapiswa kato folse

    public string Type { get; set; } = null!; 

    public int ReferenceId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}