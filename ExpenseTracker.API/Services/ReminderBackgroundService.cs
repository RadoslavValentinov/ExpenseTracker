using ExpenseTracker.Core.Interfaces;

namespace ExpenseTracker.API.Services;

public class ReminderBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public ReminderBackgroundService(
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
                using var scope =
                    _scopeFactory.CreateScope();

                var service =
                    scope.ServiceProvider
                        .GetRequiredService<IReminderService>();

                var pending =
                    await service.GetPendingForBackgroundAsync();

                foreach (var reminder in pending)
                {
                    Console.WriteLine(
                        $"🔔 REMINDER: {reminder.Title}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"Reminder background service error: {ex.Message}");
            }

            await Task.Delay(
                TimeSpan.FromSeconds(10),
                stoppingToken);
        }
    }
}