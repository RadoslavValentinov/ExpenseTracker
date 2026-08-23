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

        var paid =
            await _service.MarkAsPaidAsync(
                id,
                userId);

        if (!paid)
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

        var updated =
            await _service.UpdateAsync(
                expense,
                userId);

        if (!updated)
            return NotFound();

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