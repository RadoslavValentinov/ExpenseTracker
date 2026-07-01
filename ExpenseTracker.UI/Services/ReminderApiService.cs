using ExpenseTracker.Core.Models;

public class ReminderApiService
{
    private readonly HttpClient _http;


    public ReminderApiService(IHttpClientFactory factory)
    {
        _http = factory.CreateClient("Api");
    }

    public async Task<List<Reminder>> GetAllAsync()
    {
        var response = await _http.GetAsync("api/reminders");

        var content = await response.Content.ReadAsStringAsync();

        Console.WriteLine(content);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<List<Reminder>>() ?? new();
    }

    public async Task<List<Reminder>> GetCompletedAsync()
    {
        return await _http.GetFromJsonAsync<List<Reminder>>(
            "api/reminders/completed") ?? new();
    }

    public async Task TriggerAsync(int id)
    {
        await _http.PutAsync($"api/reminders/{id}/trigger", null);
    }

    public async Task CreateAsync(Reminder reminder)
    {
        var response = await _http.PostAsJsonAsync(
            "api/reminders",
            reminder);

        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteAsync(int id)
    {
        await _http.DeleteAsync($"api/reminders/{id}");
    }


    public async Task UpdateAsync(Reminder reminder)
    {
        var response = await _http.PutAsJsonAsync(
            $"api/reminders/{reminder.Id}",
            reminder);

        response.EnsureSuccessStatusCode();
    }
}