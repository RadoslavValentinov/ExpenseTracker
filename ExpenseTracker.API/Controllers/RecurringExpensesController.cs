using ExpenseTracker.API.DTOs;
using ExpenseTracker.Core.Interfaces;
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
    public async Task<IActionResult> GetAll()
    {
        var userId = GetUserId();

        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized();

        var result =
            await _service.GetAllAsync(
                userId);

        return Ok(result);
    }

    [HttpGet("active")]
    public async Task<IActionResult> GetActive()
    {
        var userId = GetUserId();

        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized();

        var result =
            await _service.GetActiveAsync(
                userId);

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        CreateRecurringExpenseDto dto)
    {
        var userId = GetUserId();

        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized();

        if (dto == null)
            return BadRequest(
                "Invalid recurring expense data.");

        var title = dto.Title?.Trim();

        if (string.IsNullOrWhiteSpace(title))
            return BadRequest("Title is required.");

        if (dto.Amount <= 0)
            return BadRequest(
                "Amount must be greater than zero.");

        if (dto.DayOfMonth < 1 ||
            dto.DayOfMonth > 31)
        {
            return BadRequest(
                "Day of month must be between 1 and 31.");
        }

        var recurringExpense =
            new ExpenseTracker.Core.Models.RecurringExpense
            {
                Title = title,
                Amount = dto.Amount,
                DayOfMonth = dto.DayOfMonth,
                IsActive = dto.IsActive
            };

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

        if (id <= 0)
            return BadRequest(
                "Invalid recurring expense id.");

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
        UpdateRecurringExpenseDto dto)
    {
        var userId = GetUserId();

        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized();

        if (id <= 0)
            return BadRequest(
                "Invalid recurring expense id.");

        if (dto == null)
            return BadRequest(
                "Invalid recurring expense data.");

        var title = dto.Title?.Trim();

        if (string.IsNullOrWhiteSpace(title))
            return BadRequest("Title is required.");

        if (dto.Amount <= 0)
            return BadRequest(
                "Amount must be greater than zero.");

        if (dto.DayOfMonth < 1 ||
            dto.DayOfMonth > 31)
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

        existing.Title = title;
        existing.Amount = dto.Amount;
        existing.DayOfMonth = dto.DayOfMonth;
        existing.IsActive = dto.IsActive;

        await _service.UpdateAsync(
            existing,
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

        if (id <= 0)
            return BadRequest(
                "Invalid recurring expense id.");

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