using ExpenseTracker.Core.Interfaces;
using ExpenseTracker.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ExpenseTracker.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RemindersController : ControllerBase
{
    private readonly IReminderService _service;

    public RemindersController(
        IReminderService service)
    {
        _service = service;
    }


    private string? GetUserId()
    {
        return User.FindFirstValue(
            ClaimTypes.NameIdentifier);
    }


    [HttpGet]
    public async Task<ActionResult<List<Reminder>>> Get()
    {
        var userId = GetUserId();

        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized();

        var reminders =
            await _service.GetPendingAsync(
                userId);

        return Ok(reminders);
    }


    [HttpGet("completed")]
    public async Task<ActionResult<List<Reminder>>> GetCompleted()
    {
        var userId = GetUserId();

        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized();

        var reminders =
            await _service.GetCompletedAsync(
                userId);

        return Ok(reminders);
    }


    [HttpPost]
    public async Task<IActionResult> Create(
        Reminder reminder)
    {
        var userId = GetUserId();

        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized();

        await _service.AddAsync(
            reminder,
            userId);

        return Ok();
    }


    [HttpPut("{id}/trigger")]
    public async Task<IActionResult> Trigger(
        int id)
    {
        var userId = GetUserId();

        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized();

        var reminder =
            await _service.GetByIdAsync(
                id,
                userId);

        if (reminder == null)
            return NotFound();

        await _service.MarkAsTriggeredAsync(
            id,
            userId);

        return NoContent();
    }


    [HttpPut("{id}/read")]
    public async Task<IActionResult> MarkAsRead(
        int id)
    {
        var userId = GetUserId();

        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized();

        var reminder =
            await _service.GetByIdAsync(
                id,
                userId);

        if (reminder == null)
            return NotFound();

        await _service.MarkAsReadAsync(
            id,
            userId);

        return NoContent();
    }


    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(
        int id)
    {
        var userId = GetUserId();

        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized();

        var reminder =
            await _service.GetByIdAsync(
                id,
                userId);

        if (reminder == null)
            return NotFound();

        await _service.DeleteAsync(
            id,
            userId);

        return Ok();
    }


    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        int id,
        Reminder reminder)
    {
        var userId = GetUserId();

        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized();

        if (id != reminder.Id)
            return BadRequest();

        var existing =
            await _service.GetByIdAsync(
                id,
                userId);

        if (existing == null)
            return NotFound();

        await _service.UpdateAsync(
            reminder,
            userId);

        return Ok();
    }
}