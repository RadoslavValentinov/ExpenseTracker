using ExpenseTracker.MAUI.Data;
using ExpenseTracker.MAUI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

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
                fonts.AddFont(
                    "OpenSans-Regular.ttf",
                    "OpenSansRegular");
            });

        // =====================================================
        // BLAZOR HYBRID
        // =====================================================

        builder.Services.AddMauiBlazorWebView();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        // =====================================================
        // LOCAL SQLITE DATABASE
        // =====================================================

        var databasePath = Path.Combine(
            FileSystem.AppDataDirectory,
            "expenses.db");

        builder.Services.AddDbContextFactory<ExpenseDbContext>(
            options =>
                options.UseSqlite(
                    $"Data Source={databasePath}"));

        // =====================================================
        // LOCAL SERVICES
        // =====================================================

        builder.Services.AddScoped<LocalExpenseService>();

        // =====================================================
        // BUILD APPLICATION
        // =====================================================

        var app = builder.Build();

        // =====================================================
        // CREATE LOCAL DATABASE
        // =====================================================

        using (var scope = app.Services.CreateScope())
        {
            var factory =
                scope.ServiceProvider
                    .GetRequiredService<
                        IDbContextFactory<ExpenseDbContext>>();

            using var db =
                factory.CreateDbContext();

            db.Database.EnsureCreated();
        }

        return app;
    }
}