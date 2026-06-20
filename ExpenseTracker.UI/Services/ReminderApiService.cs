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

    public async Task TriggerAsync(int id)
    {
        await _http.PutAsync($"api/reminders/{id}/trigger", null);
    }
}