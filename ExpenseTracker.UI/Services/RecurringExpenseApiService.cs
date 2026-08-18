using ExpenseTracker.Core.Models;
using System.Net.Http.Json;

namespace ExpenseTracker.UI.Services;

public class RecurringExpenseApiService
{
    private readonly IHttpClientFactory _factory;

    public RecurringExpenseApiService(IHttpClientFactory factory)
    {
        _factory = factory;
    }


    public async Task<List<RecurringExpense>> GetAllAsync()
    {
        var client = _factory.CreateClient("Api");

        var response = await client.GetAsync(
            "api/RecurringExpenses");

        await EnsureSuccessAsync(response);

        return await response.Content
            .ReadFromJsonAsync<List<RecurringExpense>>()
            ?? new List<RecurringExpense>();
    }


    public async Task AddAsync(
        RecurringExpense recurringExpense)
    {
        var client = _factory.CreateClient("Api");

        var response = await client.PostAsJsonAsync(
            "api/RecurringExpenses",
            recurringExpense);

        await EnsureSuccessAsync(response);
    }


    public async Task GenerateAsync(int id)
    {
        var client = _factory.CreateClient("Api");

        var response = await client.PostAsync(
            $"api/RecurringExpenses/{id}/generate",
            null);

        await EnsureSuccessAsync(response);
    }


    public async Task UpdateAsync(
        RecurringExpense recurringExpense)
    {
        var client = _factory.CreateClient("Api");

        var response = await client.PutAsJsonAsync(
            $"api/RecurringExpenses/{recurringExpense.Id}",
            recurringExpense);

        await EnsureSuccessAsync(response);
    }


    public async Task DeleteAsync(int id)
    {
        var client = _factory.CreateClient("Api");

        var response = await client.DeleteAsync(
            $"api/RecurringExpenses/{id}");

        await EnsureSuccessAsync(response);
    }


    private static async Task EnsureSuccessAsync(
        HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
            return;

        var error =
            await response.Content.ReadAsStringAsync();

        if (string.IsNullOrWhiteSpace(error))
        {
            error =
                $"Request failed with status code {(int)response.StatusCode}.";
        }

        throw new Exception(error);
    }
}