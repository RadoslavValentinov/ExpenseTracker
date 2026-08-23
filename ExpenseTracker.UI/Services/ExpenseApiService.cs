using ExpenseTracker.Core.Models;
using ExpenseTracker.UI.Models;

namespace ExpenseTracker.UI.Services;

public class ExpenseApiService
{
    private readonly ApiHttpClient _api;

    public ExpenseApiService(ApiHttpClient api)
    {
        _api = api;
    }

    public async Task MarkAsPaidAsync(int id)
    {
        var response =
            await _api.PutAsync(
                $"api/Expenses/{id}/pay");

        await EnsureSuccessAsync(response);
    }

    public async Task<List<Expense>> GetExpensesAsync()
    {
        var response =
            await _api.GetAsync("api/expenses");

        await EnsureSuccessAsync(response);

        var expenses =
            await response.Content
                .ReadFromJsonAsync<List<Expense>>();

        return expenses ?? new List<Expense>();
    }

    public async Task AddExpenseAsync(
        CreateExpenseDto dto)
    {
        var response =
            await _api.PostAsJsonAsync(
                "api/Expenses",
                dto);

        await EnsureSuccessAsync(response);
    }

    public async Task UpdateAsync(
        Expense expense)
    {
        var response =
            await _api.PutAsJsonAsync(
                $"api/expenses/{expense.Id}",
                expense);

        await EnsureSuccessAsync(response);
    }

    public async Task DeleteAsync(int id)
    {
        var response =
            await _api.DeleteAsync(
                $"api/expenses/{id}");

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