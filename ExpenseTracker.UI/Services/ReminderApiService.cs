using ExpenseTracker.Core.Models;
using System.Net.Http.Json;

namespace ExpenseTracker.UI.Services;

public class ReminderApiService
{
    private readonly ApiHttpClient _api;

    public ReminderApiService(ApiHttpClient api)
    {
        _api = api;
    }


    public async Task<List<Reminder>> GetAllAsync()
    {
        var response =
            await _api.GetAsync(
                "api/reminders");

        await EnsureSuccessAsync(response);

        return await response.Content
            .ReadFromJsonAsync<List<Reminder>>()
            ?? new List<Reminder>();
    }


    public async Task<List<Reminder>> GetCompletedAsync()
    {
        var response =
            await _api.GetAsync(
                "api/reminders/completed");

        await EnsureSuccessAsync(response);

        return await response.Content
            .ReadFromJsonAsync<List<Reminder>>()
            ?? new List<Reminder>();
    }


    public async Task TriggerAsync(int id)
    {
        var response =
            await _api.PutAsync(
                $"api/reminders/{id}/trigger");

        await EnsureSuccessAsync(response);
    }


    public async Task CreateAsync(
        Reminder reminder)
    {
        var response =
            await _api.PostAsJsonAsync(
                "api/reminders",
                reminder);

        await EnsureSuccessAsync(response);
    }


    public async Task DeleteAsync(int id)
    {
        var response =
            await _api.DeleteAsync(
                $"api/reminders/{id}");

        await EnsureSuccessAsync(response);
    }


    public async Task MarkAsReadAsync(int id)
    {
        var response =
            await _api.PutAsync(
                $"api/reminders/{id}/read");

        await EnsureSuccessAsync(response);
    }


    public async Task UpdateAsync(
        Reminder reminder)
    {
        var response =
            await _api.PutAsJsonAsync(
                $"api/reminders/{reminder.Id}",
                reminder);

        await EnsureSuccessAsync(response);
    }


    private static async Task EnsureSuccessAsync(
        HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
            return;

        var error =
            await response.Content
                .ReadAsStringAsync();

        if (string.IsNullOrWhiteSpace(error))
        {
            error =
                $"Request failed with status code {(int)response.StatusCode}.";
        }

        throw new Exception(error);
    }
}