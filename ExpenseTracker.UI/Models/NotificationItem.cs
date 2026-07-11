namespace ExpenseTracker.UI.Models;

public class NotificationItem
{
    public int Id { get; set; }

    public string Title { get; set; } = "";

    public string Description { get; set; } = "";

    public string Type { get; set; } = "";

    public DateTime Date { get; set; }

    public string Url { get; set; } = "";

    public bool IsRead { get; set; }
}