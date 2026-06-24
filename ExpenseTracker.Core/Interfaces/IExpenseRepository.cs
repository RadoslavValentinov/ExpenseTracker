using ExpenseTracker.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.Core.Interfaces
{
    public interface IExpenseRepository
    {
        Task AddAsync(Expense expense);
        Task<List<Expense>> GetAllAsync();
        Task<Expense?> GetByIdAsync(int id);
        Task UpdateAsync(Expense expense);
        Task DeleteAsync(int id);
        Task<List<Expense>> GetByMonthAsync(int month);
        Task MarkAsPaidAsync(int id);
    }
}
