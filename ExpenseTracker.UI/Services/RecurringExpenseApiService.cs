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

        return await client.GetFromJsonAsync<List<RecurringExpense>>(
            "api/RecurringExpenses")
            ?? new List<RecurringExpense>();
    }

    public async Task AddAsync(RecurringExpense recurringExpense)
    {
        var client = _factory.CreateClient("Api");

        await client.PostAsJsonAsync(
            "api/RecurringExpenses",
            recurringExpense);
    }

    public async Task GenerateAsync(int id)
    {
        var client = _factory.CreateClient("Api");

        await client.PostAsync(
            $"api/RecurringExpenses/{id}/generate",
            null);
    }

    public async Task UpdateAsync(RecurringExpense recurringExpense)
    {
        var client = _factory.CreateClient("Api");

        await client.PutAsJsonAsync(
            $"api/RecurringExpenses/{recurringExpense.Id}",
             recurringExpense);
    }


    public async Task DeleteAsync(int id)
    {
        var client = _factory.CreateClient("Api");

        await client.DeleteAsync(
            $"api/RecurringExpenses/{id}");
    }
}