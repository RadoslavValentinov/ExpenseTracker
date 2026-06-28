public interface ITaskService
{
    Task AddTaskAsync(TaskItem task);

    Task<List<TaskItem>> GetTasksAsync();

    Task CompleteTaskAsync(int id);

    Task UpdateAsync(TaskItem task);

    Task DeleteAsync(int id);
}