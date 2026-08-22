using ExpenseTracker.Core.Interfaces;
using ExpenseTracker.Core.Models;
using ExpenseTracker.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Infrastructure.Repositories;

public class TaskRepository : ITaskRepository
{
    private readonly AppDbContext _context;

    public TaskRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(TaskItem task)
    {
        await _context.Tasks.AddAsync(task);

        await _context.SaveChangesAsync();
    }


    public async Task<List<TaskItem>> GetAllAsync(
        string userId)
    {
        return await _context.Tasks
            .Where(x => x.UserId == userId)
            .ToListAsync();
    }


    public async Task<TaskItem?> GetByIdAsync(
        int id,
        string userId)
    {
        return await _context.Tasks
            .FirstOrDefaultAsync(x =>
                x.Id == id &&
                x.UserId == userId);
    }


    public async Task UpdateAsync(TaskItem task)
    {
        _context.Tasks.Update(task);

        await _context.SaveChangesAsync();
    }


    public async Task DeleteAsync(
        int id,
        string userId)
    {
        var task = await _context.Tasks
            .FirstOrDefaultAsync(x =>
                x.Id == id &&
                x.UserId == userId);

        if (task == null)
            return;

        _context.Tasks.Remove(task);

        await _context.SaveChangesAsync();
    }
}