using ExpenseTracker.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace ExpenseTracker.API.Services;

public class RecurringExpenseBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public RecurringExpenseBackgroundService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _scopeFactory.CreateScope();

            var recurringService =
                scope.ServiceProvider.GetRequiredService<IRecurringExpenseService>();

            var expenseService =
                scope.ServiceProvider.GetRequiredService<IExpenseService>();

            var recurringExpenses = await recurringService.GetActiveAsync();

            var today = DateTime.UtcNow.Date;

            foreach (var recurring in recurringExpenses)
            {

                if (today.Day != recurring.DayOfMonth)
                    continue;

            
                if (recurring.LastGeneratedDate.HasValue &&
                    recurring.LastGeneratedDate.Value.Year == today.Year &&
                    recurring.LastGeneratedDate.Value.Month == today.Month)
                {
                    continue;
                }

            
                var expense = new ExpenseTracker.Core.Models.Expense
                {
                    Title = recurring.Title,
                    Amount = recurring.Amount,
                    DueDate = today,
                    IsPaid = false
                };

                await expenseService.AddExpenseAsync(expense);

                recurring.LastGeneratedDate = today;
                await recurringService.UpdateAsync(recurring);
            }


            await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
        }
    }
}