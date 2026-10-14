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

        if (dto == null)
            return BadRequest("Invalid task data.");

        var title = dto.Title?.Trim();

        if (string.IsNullOrWhiteSpace(title))
            return BadRequest("Title is required.");

        if (dto.DueDate == DateTime.MinValue)
            return BadRequest("Due date is required.");

        if (dto.Priority < 0 || dto.Priority > 5)
            return BadRequest(
                "Priority must be between 0 and 5.");

        if (dto.ReminderTime.HasValue &&
            dto.ReminderTime.Value == DateTime.MinValue)
        {
            return BadRequest("Invalid reminder time.");
        }

        var task = new TaskItem
        {
            Title = title,
            Description =
                string.IsNullOrWhiteSpace(dto.Description)
                    ? null
                    : dto.Description.Trim(),
            DueDate = dto.DueDate,
            Priority = dto.Priority,
            ReminderTime = dto.ReminderTime,
            RepeatMonthly = dto.RepeatMonthly,
            IsCompleted = false,
            HasReminder = dto.ReminderTime.HasValue
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

        if (id <= 0)
            return BadRequest("Invalid task id.");

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
        UpdateTaskDto dto)
    {
        var userId = GetUserId();

        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized();

        if (id <= 0)
            return BadRequest("Invalid task id.");

        if (dto == null)
            return BadRequest("Invalid task data.");

        var title = dto.Title?.Trim();

        if (string.IsNullOrWhiteSpace(title))
            return BadRequest("Title is required.");

        if (dto.DueDate == DateTime.MinValue)
            return BadRequest("Due date is required.");

        if (dto.Priority < 0 || dto.Priority > 5)
            return BadRequest(
                "Priority must be between 0 and 5.");

        var task = new TaskItem
        {
            Id = id,
            Title = title,
            Description =
                string.IsNullOrWhiteSpace(dto.Description)
                    ? null
                    : dto.Description.Trim(),
            DueDate = dto.DueDate,
            Priority = dto.Priority,
            ReminderTime = dto.ReminderTime,
            RepeatMonthly = dto.RepeatMonthly
        };

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

        if (id <= 0)
            return BadRequest("Invalid task id.");

        var deleted =
            await _service.DeleteAsync(
                id,
                userId);

        if (!deleted)
            return NotFound();

        return Ok();
    }
}