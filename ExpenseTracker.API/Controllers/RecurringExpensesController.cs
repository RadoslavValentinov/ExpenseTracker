using ExpenseTracker.Core.Interfaces;
using ExpenseTracker.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ExpenseTracker.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RecurringExpensesController : ControllerBase
{
    private readonly IRecurringExpenseService _service;

    public RecurringExpensesController(
        IRecurringExpenseService service)
    {
        _service = service;
    }


    private string? GetUserId()
    {
        return User.FindFirstValue(
            ClaimTypes.NameIdentifier);
    }


    [HttpGet]
    public async Task<ActionResult<List<RecurringExpense>>> GetAll()
    {
        var userId = GetUserId();

        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized();

        var items =
            await _service.GetAllAsync(
                userId);

        return Ok(items);
    }


    [HttpPost]
    public async Task<IActionResult> Create(
        RecurringExpense recurringExpense)
    {
        var userId = GetUserId();

        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized();


        if (recurringExpense == null)
            return BadRequest(
                "Invalid recurring expense data.");


        if (string.IsNullOrWhiteSpace(
                recurringExpense.Title))
        {
            return BadRequest(
                "Title is required.");
        }


        if (recurringExpense.Amount <= 0)
        {
            return BadRequest(
                "Amount must be greater than zero.");
        }


        if (recurringExpense.DayOfMonth < 1 ||
            recurringExpense.DayOfMonth > 31)
        {
            return BadRequest(
                "Day of month must be between 1 and 31.");
        }


        await _service.AddAsync(
            recurringExpense,
            userId);

        return Ok();
    }


    [HttpPost("{id}/generate")]
    public async Task<IActionResult> Generate(
        int id)
    {
        var userId = GetUserId();

        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized();


        var generated =
            await _service.GenerateExpenseForUserAsync(
                id,
                userId);


        if (!generated)
            return NotFound();


        return Ok();
    }


    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        int id,
        RecurringExpense recurringExpense)
    {
        var userId = GetUserId();

        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized();


        if (recurringExpense == null)
            return BadRequest(
                "Invalid recurring expense data.");


        if (id != recurringExpense.Id)
            return BadRequest();


        if (string.IsNullOrWhiteSpace(
                recurringExpense.Title))
        {
            return BadRequest(
                "Title is required.");
        }


        if (recurringExpense.Amount <= 0)
        {
            return BadRequest(
                "Amount must be greater than zero.");
        }


        if (recurringExpense.DayOfMonth < 1 ||
            recurringExpense.DayOfMonth > 31)
        {
            return BadRequest(
                "Day of month must be between 1 and 31.");
        }


        var existing =
            await _service.GetByIdAsync(
                id,
                userId);

        if (existing == null)
            return NotFound();


        await _service.UpdateAsync(
            recurringExpense,
            userId);

        return Ok();
    }


    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(
        int id)
    {
        var userId = GetUserId();

        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized();


        var existing =
            await _service.GetByIdAsync(
                id,
                userId);

        if (existing == null)
            return NotFound();


        await _service.DeleteAsync(
            id,
            userId);

        return NoContent();
    }
}