using ExpenseTracker.Core.Models;
using ExpenseTracker.UI.Models;
using System.Net.Http.Json;

namespace ExpenseTracker.UI.Services;

public class ExpenseApiService
{
    private readonly IHttpClientFactory _factory;

    public ExpenseApiService(IHttpClientFactory factory)
    {
        _factory = factory;
    }


    public async Task MarkAsPaidAsync(int id)
    {
        var client = _factory.CreateClient("Api");

        var response = await client.PutAsync(
            $"api/Expenses/{id}/pay",
            null);

        await EnsureSuccessAsync(response);
    }


    public async Task<List<Expense>> GetExpensesAsync()
    {
        var client = _factory.CreateClient("Api");

        var response = await client.GetAsync(
            "api/expenses");

        await EnsureSuccessAsync(response);

        var expenses =
            await response.Content.ReadFromJsonAsync<List<Expense>>();

        return expenses ?? new List<Expense>();
    }


    public async Task AddExpenseAsync(CreateExpenseDto dto)
    {
        var client = _factory.CreateClient("Api");

        var response = await client.PostAsJsonAsync(
            "api/Expenses",
            dto);

        await EnsureSuccessAsync(response);
    }


    public async Task UpdateAsync(Expense expense)
    {
        var client = _factory.CreateClient("Api");

        var response = await client.PutAsJsonAsync(
            $"api/expenses/{expense.Id}",
            expense);

        await EnsureSuccessAsync(response);
    }


    public async Task DeleteAsync(int id)
    {
        var client = _factory.CreateClient("Api");

        var response = await client.DeleteAsync(
            $"api/expenses/{id}");

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