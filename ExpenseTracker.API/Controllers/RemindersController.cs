using ExpenseTracker.Core.Interfaces;
using ExpenseTracker.Core.Models;
using ExpenseTracker.Core.Services;
using Microsoft.AspNetCore.Mvc;

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
}