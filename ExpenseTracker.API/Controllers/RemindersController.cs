using ExpenseTracker.Core.Interfaces;
using ExpenseTracker.Core.Models;
using ExpenseTracker.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RemindersController : ControllerBase
{
    private readonly IReminderService _service;

    public RemindersController(IReminderService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<List<Reminder>>> Get()
    {
        var reminders = await _service.GetPendingAsync();

        return Ok(reminders);
    }

    [HttpGet("completed")]
    public async Task<ActionResult<List<Reminder>>> GetCompleted()
    {
        return Ok(await _service.GetCompletedAsync());
    }

    [HttpPost]
    public async Task<IActionResult> Create(Reminder reminder)
    {
        await _service.AddAsync(reminder);

        return Ok();
    }

    [HttpPut("{id}/trigger")]
    public async Task<IActionResult> Trigger(int id)
    {
        await _service.MarkAsTriggeredAsync(id);

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id);

        return Ok();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
    int id,
    Reminder reminder)
    {
        if (id != reminder.Id)
            return BadRequest();

        await _service.UpdateAsync(reminder);

        return Ok();
    }

   
}