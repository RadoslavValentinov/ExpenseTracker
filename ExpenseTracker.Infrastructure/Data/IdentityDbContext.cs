using ExpenseTracker.Core.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Infrastructure.Data;

public class ApplicationIdentityDbContext
    : IdentityDbContext<ApplicationUser>
{
    public ApplicationIdentityDbContext(
        DbContextOptions<ApplicationIdentityDbContext> options)
        : base(options)
    {
    }
}