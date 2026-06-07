using ExpenseTracker.Core.Models;

public class ReminderApiService
{
    private readonly HttpClient _http;

    public ReminderApiService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<Reminder>> GetAllAsync()
    {
        return await _http.GetFromJsonAsync<List<Reminder>>("api/reminders")
               ?? new();
    }

    public async Task TriggerAsync(int id)
    {
        await _http.PutAsync($"api/reminders/{id}/trigger", null);
    }
}