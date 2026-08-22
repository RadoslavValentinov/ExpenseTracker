using ExpenseTracker.Core.Interfaces;
using ExpenseTracker.Core.Models;
using ExpenseTracker.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Infrastructure.Repositories;

public class ExpenseRepository : IExpenseRepository
{
    private readonly AppDbContext _context;

    public ExpenseRepository(AppDbContext context)
    {
        _context = context;
    }


    public async Task AddAsync(Expense expense)
    {
        await _context.Expenses.AddAsync(expense);

        await _context.SaveChangesAsync();
    }


    public async Task<List<Expense>> GetAllAsync(
        string userId)
    {
        return await _context.Expenses
            .Where(x => x.UserId == userId)
            .ToListAsync();
    }


    public async Task<Expense?> GetByIdAsync(
        int id,
        string userId)
    {
        return await _context.Expenses
            .FirstOrDefaultAsync(x =>
                x.Id == id &&
                x.UserId == userId);
    }


    public async Task UpdateAsync(Expense expense)
    {
        _context.Expenses.Update(expense);

        await _context.SaveChangesAsync();
    }


    public async Task DeleteAsync(
        int id,
        string userId)
    {
        var expense =
            await _context.Expenses
                .FirstOrDefaultAsync(x =>
                    x.Id == id &&
                    x.UserId == userId);

        if (expense == null)
            return;

        _context.Expenses.Remove(expense);

        await _context.SaveChangesAsync();
    }


    public async Task<List<Expense>> GetByMonthAsync(
        int month,
        string userId)
    {
        return await _context.Expenses
            .Where(x =>
                x.UserId == userId &&
                x.DueDate.Month == month)
            .ToListAsync();
    }


    public async Task MarkAsPaidAsync(
        int id,
        string userId)
    {
        var expense =
            await _context.Expenses
                .FirstOrDefaultAsync(x =>
                    x.Id == id &&
                    x.UserId == userId);

        if (expense == null)
            return;

        expense.IsPaid = true;

        await _context.SaveChangesAsync();
    }
}