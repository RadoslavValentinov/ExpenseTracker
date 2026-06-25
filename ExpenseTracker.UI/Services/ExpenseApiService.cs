using ExpenseTracker.Core.Models;
using ExpenseTracker.UI.Models;
using System.Net.Http.Json;
using static System.Net.WebRequestMethods;

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

        await client.PutAsync(
            $"api/Expenses/{id}/pay",
            null);
    }

    public async Task<List<Expense>> GetExpensesAsync()
    {
        var client = _factory.CreateClient("Api");

        var expenses = await client.GetFromJsonAsync<List<Expense>>(
            "api/expenses");

        return expenses ?? new List<Expense>();
    }

    public async Task AddExpenseAsync(CreateExpenseDto dto)
    {
        var client = _factory.CreateClient("Api");

        var response = await client.PostAsJsonAsync(
            "api/Expenses",
            dto);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();

            throw new Exception(error);
        }
    }

    public async Task UpdateAsync(Expense expense)
    {
        var client = _factory.CreateClient("Api");

        await client.PutAsJsonAsync(
            $"api/expenses/{expense.Id}",
            expense);
    }

    public async Task DeleteAsync(int id)
    {
        var client = _factory.CreateClient("Api");

        await client.DeleteAsync($"api/expenses/{id}");
    }
}