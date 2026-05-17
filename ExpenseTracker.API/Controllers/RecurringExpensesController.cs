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
}