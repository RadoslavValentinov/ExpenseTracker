using ExpenseTracker.API.DTOs;
using ExpenseTracker.Core.Interfaces;
using ExpenseTracker.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ExpenseTracker.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TasksController : ControllerBase
{
    private readonly ITaskService _service;

    public TasksController(
        ITaskService service)
    {
        _service = service;
    }


    private string? GetUserId()
    {
        return User.FindFirstValue(
            ClaimTypes.NameIdentifier);
    }


    [HttpPost]
    public async Task<IActionResult> Create(
        CreateTaskDto dto)
    {
        var userId = GetUserId();

        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized();

        var task = new TaskItem
        {
            Title = dto.Title,
            Description = dto.Description,
            DueDate = dto.DueDate,
            Priority = dto.Priority,
            ReminderTime = dto.ReminderTime,
            IsCompleted = false
        };

        await _service.AddTaskAsync(
            task,
            userId);

        return Ok();
    }


    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var userId = GetUserId();

        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized();

        var tasks =
            await _service.GetTasksAsync(
                userId);

        return Ok(tasks);
    }


    [HttpPut("{id}/complete")]
    public async Task<IActionResult> Complete(
        int id)
    {
        var userId = GetUserId();

        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized();

        var completed =
            await _service.CompleteTaskAsync(
                id,
                userId);

        if (!completed)
            return NotFound();

        return Ok();
    }


    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        int id,
        TaskItem task)
    {
        var userId = GetUserId();

        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized();

        if (id != task.Id)
            return BadRequest();

        var updated =
            await _service.UpdateAsync(
                task,
                userId);

        if (!updated)
            return NotFound();

        return Ok();
    }


    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(
        int id)
    {
        var userId = GetUserId();

        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized();

        var deleted =
            await _service.DeleteAsync(
                id,
                userId);

        if (!deleted)
            return NotFound();

        return Ok();
    }
}