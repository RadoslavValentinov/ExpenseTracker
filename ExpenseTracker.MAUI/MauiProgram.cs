using ExpenseTracker.MAUI.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ExpenseTracker.MAUI.Services;

namespace ExpenseTracker.MAUI;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        builder.Services.AddMauiBlazorWebView();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        var databasePath = Path.Combine(
            FileSystem.AppDataDirectory,
            "expenses.db");

        builder.Services.AddDbContextFactory<ExpenseDbContext>(
            options =>
            {
                options.UseSqlite($"Data Source={databasePath}");
            });

        builder.Services.AddScoped<LocalExpenseService>();

        var app = builder.Build();

        using (var scope = app.Services.CreateScope())
        {
            var factory = scope.ServiceProvider
                .GetRequiredService<IDbContextFactory<ExpenseDbContext>>();

            using var db = factory.CreateDbContext();

            db.Database.EnsureCreated();
        }

        return app;
    }
}