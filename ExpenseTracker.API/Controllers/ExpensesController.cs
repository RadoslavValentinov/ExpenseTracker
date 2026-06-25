using ExpenseTracker.API.DTOs;
using ExpenseTracker.Core.Interfaces;
using ExpenseTracker.Core.Models;
using ExpenseTracker.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ExpensesController : ControllerBase
{
    private readonly IExpenseService _service;

    public ExpensesController(IExpenseService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> AddExpense(CreateExpenseDto dto)
    {
        var expense = new Expense
        {
            Title = dto.Title,
            Amount = dto.Amount,
            DueDate = dto.DueDate,
            Category = dto.Category,
            IsPaid = false
        };

        await _service.AddExpenseAsync(expense);

        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> GetExpenses()
    {
        var expenses = await _service.GetExpensesAsync();
        return Ok(expenses);
    }


    [HttpPut("{id}/pay")]
    public async Task<IActionResult> MarkAsPaid(int id)
    {
        var expense = await _service.GetExpensesAsync();

        var existing = expense.FirstOrDefault(e => e.Id == id);

        if (existing == null)
            return NotFound($"Expense with ID {id} not found");

        await _service.MarkAsPaidAsync(id);

        return Ok();
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
      Expense expense)
    {
        if (id != expense.Id)
            return BadRequest();

        await _service.UpdateAsync(expense);

        return Ok();
    }

    [HttpGet("month/{month}")]
    public async Task<IActionResult> GetByMonth(int month)
    {
        var result = await _service.GetByMonthAsync(month);
        return Ok(result);
    }

}