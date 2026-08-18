using ExpenseTracker.Core.Models;
using ExpenseTracker.UI.Models;
using System.Net.Http.Json;

namespace ExpenseTracker.UI.Services;

public class TaskApiService
{
    private readonly IHttpClientFactory _factory;

    public TaskApiService(IHttpClientFactory factory)
    {
        _factory = factory;
    }


    public async Task<List<TaskItem>?> GetTasksAsync()
    {
        var client = _factory.CreateClient("Api");

        var response = await client.GetAsync(
            "api/tasks");

        await EnsureSuccessAsync(response);

        return await response.Content
            .ReadFromJsonAsync<List<TaskItem>>();
    }


    public async Task AddTaskAsync(CreateTaskDto dto)
    {
        var client = _factory.CreateClient("Api");

        var response = await client.PostAsJsonAsync(
            "api/tasks",
            dto);

        await EnsureSuccessAsync(response);
    }


    public async Task CompleteTaskAsync(int id)
    {
        var client = _factory.CreateClient("Api");

        var response = await client.PutAsync(
            $"api/tasks/{id}/complete",
            null);

        await EnsureSuccessAsync(response);
    }


    public async Task UpdateAsync(TaskItem task)
    {
        var client = _factory.CreateClient("Api");

        var response = await client.PutAsJsonAsync(
            $"api/tasks/{task.Id}",
            task);

        await EnsureSuccessAsync(response);
    }


    public async Task DeleteAsync(int id)
    {
        var client = _factory.CreateClient("Api");

        var response = await client.DeleteAsync(
            $"api/tasks/{id}");

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