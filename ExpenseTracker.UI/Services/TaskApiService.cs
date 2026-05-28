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

        return await client.GetFromJsonAsync<List<TaskItem>>(
            "api/tasks");
    }

    public async Task AddTaskAsync(CreateTaskDto dto)
    {
        var client = _factory.CreateClient("Api");

        await client.PostAsJsonAsync(
            "api/tasks",
            dto);
    }

    public async Task CompleteTaskAsync(int id)
    {
        var client = _factory.CreateClient("Api");

        await client.PutAsync(
            $"api/tasks/{id}/complete",
            null);
    }
}