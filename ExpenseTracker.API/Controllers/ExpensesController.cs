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
public class ExpensesController : ControllerBase
{
    private readonly IExpenseService _service;

    public ExpensesController(
        IExpenseService service)
    {
        _service = service;
    }


    private string? GetUserId()
    {
        return User.FindFirstValue(
            ClaimTypes.NameIdentifier);
    }


    [HttpPost]
    public async Task<IActionResult> AddExpense(
        CreateExpenseDto dto)
    {
        var userId = GetUserId();

        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized();

        var expense = new Expense
        {
            Title = dto.Title,
            Amount = dto.Amount,
            DueDate = dto.DueDate,
            Category = dto.Category,
            IsPaid = false
        };

        await _service.AddExpenseAsync(
            expense,
            userId);

        return Ok();
    }


    [HttpGet]
    public async Task<IActionResult> GetExpenses()
    {
        var userId = GetUserId();

        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized();

        var expenses =
            await _service.GetExpensesAsync(
                userId);

        return Ok(expenses);
    }


    [HttpPut("{id}/pay")]
    public async Task<IActionResult> MarkAsPaid(
        int id)
    {
        var userId = GetUserId();

        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized();

        var existing =
            await _service.GetExpensesAsync(
                userId);

        var expense =
            existing.FirstOrDefault(x =>
                x.Id == id);

        if (expense == null)
            return NotFound(
                $"Expense with ID {id} not found");

        await _service.MarkAsPaidAsync(
            id,
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
            await _service.GetExpensesAsync(
                userId);

        if (!existing.Any(x => x.Id == id))
            return NotFound(
                $"Expense with ID {id} not found");

        await _service.DeleteAsync(
            id,
            userId);

        return Ok();
    }


    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        int id,
        Expense expense)
    {
        var userId = GetUserId();

        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized();

        if (id != expense.Id)
            return BadRequest();

        var existing =
            await _service.GetExpensesAsync(
                userId);

        if (!existing.Any(x => x.Id == id))
            return NotFound(
                $"Expense with ID {id} not found");

        await _service.UpdateAsync(
            expense,
            userId);

        return Ok();
    }


    [HttpGet("month/{month}")]
    public async Task<IActionResult> GetByMonth(
        int month)
    {
        var userId = GetUserId();

        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized();

        var result =
            await _service.GetByMonthAsync(
                month,
                userId);

        return Ok(result);
    }
}