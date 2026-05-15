using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using ExpenseTracker.Core.Models;
using ExpenseTracker.Infrastructure.Data;
using ExpenseTracker.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory;
using Microsoft.VSDiagnostics;

namespace ExpenseTracker.Benchmarks
{
    [CPUUsageDiagnoser]
    public class ReminderRepositoryBenchmarks
    {
        private List<Reminder> _seedReminders = null !;
        private const int SeedCount = 10000;
        [GlobalSetup]
        public void GlobalSetup()
        {
            _seedReminders = new List<Reminder>(SeedCount);
            var now = DateTime.UtcNow;
            for (int i = 0; i < SeedCount; i++)
            {
                _seedReminders.Add(new Reminder { Id = i + 1, Title = "Reminder " + i, ReminderDate = now.AddMinutes(-i % 60), IsTriggered = (i % 5 == 0) // some already triggered
 });
            }
        }

        private AppDbContext CreateContextAndSeed(string dbName)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase(dbName).Options;
            var context = new AppDbContext(options);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            context.Reminders.AddRange(_seedReminders.Select(r => new Reminder { Title = r.Title, ReminderDate = r.ReminderDate, IsTriggered = r.IsTriggered }));
            context.SaveChanges();
            return context;
        }

        [Benchmark]
        public async Task GetPendingAsync_Current()
        {
            var dbName = Guid.NewGuid().ToString();
            await using var context = CreateContextAndSeed(dbName);
            var repo = new ReminderRepository(context);
            var pending = await repo.GetPendingAsync();
            // touch the result
            var count = pending.Count;
        }

        [Benchmark]
        public async Task MarkAsTriggeredAsync_Current()
        {
            var dbName = Guid.NewGuid().ToString();
            await using var context = CreateContextAndSeed(dbName);
            var repo = new ReminderRepository(context);
            var pending = await repo.GetPendingAsync();
            if (pending.Count == 0)
                return;
            var id = pending[0].Id;
            await repo.MarkAsTriggeredAsync(id);
        }
    }
}