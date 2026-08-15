using ExpenseTracker.Core.Interfaces;
using ExpenseTracker.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RecurringExpensesController : ControllerBase
{
    private readonly IRecurringExpenseService _service;

    public RecurringExpensesController(IRecurringExpenseService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<List<RecurringExpense>>> GetAll()
    {
        var items = await _service.GetAllAsync();
        return Ok(items);
    }

    [HttpPost]
    public async Task<IActionResult> Create(RecurringExpense recurringExpense)
    {
        await _service.AddAsync(recurringExpense);
        return Ok();
    }

    [HttpPost("{id}/generate")]
    public async Task<IActionResult> Generate(int id)
    {
        await _service.GenerateExpenseAsync(id);

        return Ok();
    }


    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
    int id,
    RecurringExpense recurringExpense)
    {
        if (id != recurringExpense.Id)
            return BadRequest();

        await _service.UpdateAsync(recurringExpense);

        return Ok();
    }


    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id);

        return NoContent();
    }

}