using ExpenseTracker.API.Services;
using ExpenseTracker.Core.Interfaces;
using ExpenseTracker.Core.Models;
using ExpenseTracker.Core.Services;
using ExpenseTracker.Infrastructure.Data;
using ExpenseTracker.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);


// =========================
// CONTROLLERS / SWAGGER
// =========================

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your JWT token."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});


// =========================
// APPLICATION DATABASE
// =========================

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(
        builder.Configuration.GetConnectionString("DefaultConnection")));


// =========================
// IDENTITY DATABASE
// =========================

builder.Services.AddDbContext<ApplicationIdentityDbContext>(options =>
    options.UseSqlite(
        builder.Configuration.GetConnectionString("DefaultConnection")));


// =========================
// JWT AUTHENTICATION
// =========================

var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException(
        "JWT Key is not configured.");

var jwtIssuer = builder.Configuration["Jwt:Issuer"]
    ?? throw new InvalidOperationException(
        "JWT Issuer is not configured.");

var jwtAudience = builder.Configuration["Jwt:Audience"]
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

            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,

            IssuerSigningKey =
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtKey))
        };

    // =========================
    // JWT DIAGNOSTICS
    // =========================

    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine("=================================");
            Console.WriteLine("JWT AUTHENTICATION FAILED");
            Console.WriteLine($"Exception: {context.Exception}");
            Console.WriteLine($"Message: {context.Exception.Message}");
            Console.WriteLine("=================================");

            return Task.CompletedTask;
        }
    };
});


// =========================
// IDENTITY
// =========================

builder.Services.AddSingleton(TimeProvider.System);

builder.Services.AddIdentityCore<ApplicationUser>()
    .AddEntityFrameworkStores<ApplicationIdentityDbContext>()
    .AddSignInManager();


// =========================
// REPOSITORIES / SERVICES
// =========================

builder.Services.AddScoped<IExpenseRepository, ExpenseRepository>();
builder.Services.AddScoped<IExpenseService, ExpenseService>();

builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<ITaskService, TaskService>();

builder.Services.AddScoped<IReminderRepository, ReminderRepository>();
builder.Services.AddScoped<IReminderService, ReminderService>();

builder.Services.AddScoped<IRecurringExpenseRepository, RecurringExpenseRepository>();
builder.Services.AddScoped<IRecurringExpenseService, RecurringExpenseService>();


// =========================
// BACKGROUND SERVICES
// =========================

builder.Services.AddHostedService<ReminderBackgroundService>();

builder.Services.AddHostedService<RecurringExpenseBackgroundService>();


var app = builder.Build();


// =========================
// DEVELOPMENT
// =========================

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI();
}


// =========================
// MIDDLEWARE
// =========================

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();