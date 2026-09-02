using ExpenseTracker.UI.Components;
using ExpenseTracker.UI.Services;
using Microsoft.AspNetCore.Components.Authorization;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);


        // =========================
        // BLAZOR
        // =========================

        builder.Services
            .AddRazorComponents()
            .AddInteractiveServerComponents();


        // =========================
        // BLAZOR AUTHORIZATION
        // =========================

        builder.Services.AddAuthorizationCore();

        builder.Services.AddCascadingAuthenticationState();


        // =========================
        // AUTH SERVICES
        // =========================

        builder.Services.AddScoped<AuthService>();

        builder.Services.AddScoped<AuthStateService>();

        builder.Services.AddScoped<JwtAuthenticationStateProvider>();

        builder.Services.AddScoped<AuthenticationStateProvider>(
            provider =>
                provider.GetRequiredService<
                    JwtAuthenticationStateProvider>());

        

        // =========================
        // API CLIENT
        // =========================

        builder.Services.AddScoped<ApiHttpClient>();


        // =========================
        // APPLICATION SERVICES
        // =========================

        builder.Services.AddScoped<ExpenseApiService>();

        builder.Services.AddScoped<TaskApiService>();

        builder.Services.AddScoped<ReminderApiService>();

        builder.Services.AddScoped<NotificationService>();

        builder.Services.AddScoped<RecurringExpenseApiService>();


        // =========================
        // LOGIN HTTP CLIENT
        // =========================

        builder.Services.AddHttpClient(
            "AuthApi",
            client =>
            {
                client.BaseAddress =
                    new Uri("https://localhost:7135/");
            });


        var app = builder.Build();


        // =========================
        // ERROR HANDLING
        // =========================

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler(
                "/Error",
                createScopeForErrors: true);

            app.UseHsts();
        }


        // =========================
        // MIDDLEWARE
        // =========================

        app.UseHttpsRedirection();

        app.UseStaticFiles();

        app.UseAntiforgery();


        // =========================
        // COMPONENTS
        // =========================

        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode();


        app.Run();
    }
}