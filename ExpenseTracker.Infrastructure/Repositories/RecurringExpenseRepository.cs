using ExpenseTracker.Core.Interfaces;
using ExpenseTracker.Core.Models;
using ExpenseTracker.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Infrastructure.Repositories;

public class RecurringExpenseRepository : IRecurringExpenseRepository
{
    private readonly AppDbContext _context;

    public RecurringExpenseRepository(
        AppDbContext context)
    {
        _context = context;
    }


    public async Task AddAsync(
        RecurringExpense recurringExpense)
    {
        await _context.RecurringExpenses.AddAsync(
            recurringExpense);

        await _context.SaveChangesAsync();
    }


    public async Task<List<RecurringExpense>> GetAllAsync(
        string userId)
    {
        return await _context.RecurringExpenses
            .Where(x => x.UserId == userId)
            .ToListAsync();
    }


    public async Task<List<RecurringExpense>> GetActiveAsync(
        string userId)
    {
        return await _context.RecurringExpenses
            .Where(x =>
                x.UserId == userId &&
                x.IsActive)
            .ToListAsync();
    }


    public async Task<RecurringExpense?> GetByIdAsync(
        int id,
        string userId)
    {
        return await _context.RecurringExpenses
            .FirstOrDefaultAsync(x =>
                x.Id == id &&
                x.UserId == userId);
    }


    public async Task UpdateAsync(
        RecurringExpense recurringExpense)
    {
        _context.RecurringExpenses.Update(
            recurringExpense);

        await _context.SaveChangesAsync();
    }


    public async Task DeleteAsync(
        int id,
        string userId)
    {
        var recurringExpense =
            await _context.RecurringExpenses
                .FirstOrDefaultAsync(x =>
                    x.Id == id &&
                    x.UserId == userId);

        if (recurringExpense == null)
            return;

        _context.RecurringExpenses.Remove(
            recurringExpense);

        await _context.SaveChangesAsync();
    }

    public async Task<RecurringExpense?> GetByIdForBackgroundAsync(
    int id)
    {
        return await _context.RecurringExpenses
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<List<RecurringExpense>> GetActiveForBackgroundAsync()
    {
        return await _context.RecurringExpenses
            .Where(x => x.IsActive)
            .ToListAsync();
    }

}