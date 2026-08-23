using ExpenseTracker.Core.Models;
using ExpenseTracker.UI.Models;

namespace ExpenseTracker.UI.Services;

public class TaskApiService
{
    private readonly ApiHttpClient _api;

    public TaskApiService(ApiHttpClient api)
    {
        _api = api;
    }


    public async Task<List<TaskItem>?> GetTasksAsync()
    {
        var response =
            await _api.GetAsync(
                "api/tasks");

        await EnsureSuccessAsync(response);

        return await response.Content
            .ReadFromJsonAsync<List<TaskItem>>();
    }


    public async Task AddTaskAsync(
        CreateTaskDto dto)
    {
        var response =
            await _api.PostAsJsonAsync(
                "api/tasks",
                dto);

        await EnsureSuccessAsync(response);
    }


    public async Task CompleteTaskAsync(
        int id)
    {
        var response =
            await _api.PutAsync(
                $"api/tasks/{id}/complete");

        await EnsureSuccessAsync(response);
    }


    public async Task UpdateAsync(
        TaskItem task)
    {
        var response =
            await _api.PutAsJsonAsync(
                $"api/tasks/{task.Id}",
                task);

        await EnsureSuccessAsync(response);
    }


    public async Task DeleteAsync(
        int id)
    {
        var response =
            await _api.DeleteAsync(
                $"api/tasks/{id}");

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