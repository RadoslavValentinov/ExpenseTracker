using ExpenseTracker.API.Services;
using ExpenseTracker.Core.Interfaces;
using ExpenseTracker.Core.Models;
using ExpenseTracker.Core.Services;
using ExpenseTracker.Infrastructure.Data;
using ExpenseTracker.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// =========================================================
// RATE LIMITING
// =========================================================

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode =
        StatusCodes.Status429TooManyRequests;

    // -----------------------------------------------------
    // GLOBAL LIMIT
    // 120 requests per minute per IP
    // -----------------------------------------------------
    options.GlobalLimiter =
        PartitionedRateLimiter.Create<HttpContext, string>(
            httpContext =>
                RateLimitPartition.GetFixedWindowLimiter(
                    httpContext.Connection.RemoteIpAddress?
                        .ToString()
                        ?? "unknown",
                    _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 120,
                        Window = TimeSpan.FromMinutes(1),
                        QueueLimit = 0,
                        AutoReplenishment = true
                    }));

    // -----------------------------------------------------
    // AUTH LIMIT
    // 10 requests per minute per IP
    // Used by Login/Register
    // -----------------------------------------------------
    options.AddPolicy(
        "auth",
        httpContext =>
            RateLimitPartition.GetFixedWindowLimiter(
                httpContext.Connection.RemoteIpAddress?
                    .ToString()
                    ?? "unknown",
                _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 10,
                    Window = TimeSpan.FromMinutes(1),
                    QueueLimit = 0,
                    AutoReplenishment = true
                }));

    // -----------------------------------------------------
    // REJECTION RESPONSE
    // -----------------------------------------------------
    options.OnRejected = async (
        context,
        cancellationToken) =>
    {
        context.HttpContext.Response.StatusCode =
            StatusCodes.Status429TooManyRequests;

        context.HttpContext.Response.Headers.RetryAfter =
            "60";

        context.HttpContext.Response.ContentType =
            "application/problem+json";

        await context.HttpContext.Response.WriteAsJsonAsync(
            new
            {
                title = "Too many requests.",
                status = 429,
                detail =
                    "Too many requests were sent. Please try again later."
            },
            cancellationToken);
    };
});


// =========================================================
// GLOBAL ERROR HANDLING
// =========================================================

builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = context =>
    {
        context.ProblemDetails.Status =
            StatusCodes.Status500InternalServerError;

        context.ProblemDetails.Title =
            "Something went wrong.";

        context.ProblemDetails.Detail =
            "An unexpected error occurred. Please try again later.";

        context.ProblemDetails.Extensions["traceId"] =
            context.HttpContext.TraceIdentifier;
    };
});


// =========================================================
// CORS
// =========================================================

builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "UiCors",
        policy =>
        {
            policy
                .WithOrigins(
                    "https://localhost:7259")
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});


// =========================================================
// CONTROLLERS / SWAGGER
// =========================================================

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition(
        "Bearer",
        new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Enter your JWT token."
        });

    options.AddSecurityRequirement(
        new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference =
                        new OpenApiReference
                        {
                            Type =
                                ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                },
                Array.Empty<string>()
            }
        });
});


// =========================================================
// APPLICATION DATABASE
// =========================================================

builder.Services.AddDbContext<AppDbContext>(
    options =>
        options.UseSqlite(
            builder.Configuration
                .GetConnectionString(
                    "DefaultConnection")));


// =========================================================
// IDENTITY DATABASE
// =========================================================

builder.Services.AddDbContext<ApplicationIdentityDbContext>(
    options =>
        options.UseSqlite(
            builder.Configuration
                .GetConnectionString(
                    "DefaultConnection")));


// =========================================================
// JWT AUTHENTICATION
// =========================================================

var jwtKey =
    builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException(
        "JWT Key is not configured.");

var jwtIssuer =
    builder.Configuration["Jwt:Issuer"]
    ?? throw new InvalidOperationException(
        "JWT Issuer is not configured.");

var jwtAudience =
    builder.Configuration["Jwt:Audience"]
    ?? throw new InvalidOperationException(
        "JWT Audience is not configured.");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme =
        JwtBearerDefaults.AuthenticationScheme;

    options.DefaultChallengeScheme =
        JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters =
        new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            RequireExpirationTime = true,

            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,

            IssuerSigningKey =
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtKey)),

            ClockSkew = TimeSpan.Zero
        };

    // =====================================================
    // JWT DIAGNOSTICS
    // =====================================================

    options.Events =
        new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine(
                    "JWT authentication failed.");

                Console.WriteLine(
                    $"Message: {context.Exception.Message}");

                return Task.CompletedTask;
            }
        };
});


// =========================================================
// IDENTITY
// =========================================================

builder.Services.AddSingleton(
    TimeProvider.System);


// =========================================================
// IDENTITY SECURITY SETTINGS
// =========================================================

builder.Services.Configure<IdentityOptions>(
    options =>
    {
        // -------------------------
        // PASSWORD
        // -------------------------

        options.Password.RequiredLength = 8;
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = true;


        // -------------------------
        // LOCKOUT
        // -------------------------

        options.Lockout.MaxFailedAccessAttempts = 5;

        options.Lockout.DefaultLockoutTimeSpan =
            TimeSpan.FromMinutes(5);

        options.Lockout.AllowedForNewUsers = true;


        // -------------------------
        // USER
        // -------------------------

        options.User.RequireUniqueEmail = true;
    });


builder.Services.AddIdentityCore<ApplicationUser>()
    .AddEntityFrameworkStores<ApplicationIdentityDbContext>()
    .AddSignInManager();


// =========================================================
// REPOSITORIES / SERVICES
// =========================================================

builder.Services.AddScoped<
    IExpenseRepository,
    ExpenseRepository>();

builder.Services.AddScoped<
    IExpenseService,
    ExpenseService>();

builder.Services.AddScoped<
    ITaskRepository,
    TaskRepository>();

builder.Services.AddScoped<
    ITaskService,
    TaskService>();

builder.Services.AddScoped<
    IReminderRepository,
    ReminderRepository>();

builder.Services.AddScoped<
    IReminderService,
    ReminderService>();

builder.Services.AddScoped<
    IRecurringExpenseRepository,
    RecurringExpenseRepository>();

builder.Services.AddScoped<
    IRecurringExpenseService,
    RecurringExpenseService>();


// =========================================================
// BACKGROUND SERVICES
// =========================================================

builder.Services.AddHostedService<
    ReminderBackgroundService>();

builder.Services.AddHostedService<
    RecurringExpenseBackgroundService>();


// =========================================================
// BUILD APPLICATION
// =========================================================

var app = builder.Build();


// =========================================================
// GLOBAL ERROR HANDLER
// =========================================================

app.UseExceptionHandler();


// =========================================================
// HTTPS / HSTS
// =========================================================

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();


// =========================================================
// DEVELOPMENT
// =========================================================

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI();
}


// =========================================================
// MIDDLEWARE
// =========================================================

app.UseCors("UiCors");

app.UseRateLimiter();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();