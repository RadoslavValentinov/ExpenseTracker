using ExpenseTracker.Core.Interfaces;

namespace ExpenseTracker.API.Services;

public class RecurringExpenseBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public RecurringExpenseBackgroundService(
        IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();

                var recurringService =
                    scope.ServiceProvider
                        .GetRequiredService<IRecurringExpenseService>();

                var recurringExpenses =
                    await recurringService.GetActiveAsync();

                foreach (var recurring in recurringExpenses)
                {
                    await recurringService.GenerateExpenseAsync(
                        recurring.Id);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"Recurring expense generation error: {ex.Message}");
            }

               await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
        }
    }
}