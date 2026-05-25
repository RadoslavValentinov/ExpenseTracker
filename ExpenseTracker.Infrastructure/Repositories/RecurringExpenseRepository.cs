using ExpenseTracker.Core.Interfaces;
using ExpenseTracker.Core.Models;
using ExpenseTracker.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Infrastructure.Repositories;

public class RecurringExpenseRepository : IRecurringExpenseRepository
{
    private readonly AppDbContext _context;

    public RecurringExpenseRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(RecurringExpense recurringExpense)
    {
        await _context.RecurringExpenses.AddAsync(recurringExpense);
        await _context.SaveChangesAsync();
    }

    public async Task<List<RecurringExpense>> GetAllAsync()
    {
        return await _context.RecurringExpenses.ToListAsync();
    }

    public async Task<List<RecurringExpense>> GetActiveAsync()
    {
        return await _context.RecurringExpenses
            .Where(r => r.IsActive)
            .ToListAsync();
    }

    public async Task UpdateAsync(RecurringExpense recurringExpense)
    {
        _context.RecurringExpenses.Update(recurringExpense);
        await _context.SaveChangesAsync();
    }
}