using ExpenseTracker.API.DTOs;
using ExpenseTracker.Core.Interfaces;
using ExpenseTracker.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    private readonly ITaskService _service;

    public TasksController(ITaskService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateTaskDto dto)
    {
        var task = new TaskItem
        {
            Title = dto.Title,
            Description = dto.Description,
            DueDate = dto.DueDate,
            Priority = dto.Priority,
            ReminderTime = dto.ReminderTime,
            IsCompleted = false
        };

        await _service.AddTaskAsync(task);

        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var tasks = await _service.GetTasksAsync();

        return Ok(tasks);
    }

    [HttpPut("{id}/complete")]
    public async Task<IActionResult> Complete(int id)
    {
        await _service.CompleteTaskAsync(id);

        return Ok();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
    int id,
    TaskItem task)
    {
        if (id != task.Id)
            return BadRequest();

        await _service.UpdateAsync(task);

        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id);

        return Ok();
    }

}