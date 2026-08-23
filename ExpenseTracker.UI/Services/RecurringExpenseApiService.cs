using ExpenseTracker.Core.Models;
using System.Net.Http.Json;

namespace ExpenseTracker.UI.Services;

public class RecurringExpenseApiService
{
    private readonly ApiHttpClient _api;

    public RecurringExpenseApiService(
        ApiHttpClient api)
    {
        _api = api;
    }


    public async Task<List<RecurringExpense>> GetAllAsync()
    {
        var response =
            await _api.GetAsync(
                "api/RecurringExpenses");

        await EnsureSuccessAsync(response);

        return await response.Content
            .ReadFromJsonAsync<List<RecurringExpense>>()
            ?? new List<RecurringExpense>();
    }


    public async Task AddAsync(
        RecurringExpense recurringExpense)
    {
        var response =
            await _api.PostAsJsonAsync(
                "api/RecurringExpenses",
                recurringExpense);

        await EnsureSuccessAsync(response);
    }


    public async Task GenerateAsync(int id)
    {
        var response =
            await _api.PostAsync(
                $"api/RecurringExpenses/{id}/generate");

        await EnsureSuccessAsync(response);
    }


    public async Task UpdateAsync(
        RecurringExpense recurringExpense)
    {
        var response =
            await _api.PutAsJsonAsync(
                $"api/RecurringExpenses/{recurringExpense.Id}",
                recurringExpense);

        await EnsureSuccessAsync(response);
    }


    public async Task DeleteAsync(int id)
    {
        var response =
            await _api.DeleteAsync(
                $"api/RecurringExpenses/{id}");

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