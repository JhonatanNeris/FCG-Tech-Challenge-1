using FCG.Application.Security;
using FCG.Domain.Entities;
using FCG.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace FCG.Infrastructure.Data;

public static class DatabaseSeeder
{
    private const string AdminName = "FCG Admin";
    private const string AdminEmail = "fgc_admin@admin.com";
    private const string AdminPassword = "admin";

    public static async Task SeedAsync(AppDbContext context, string secretKey)
    {
        var adminExists = await context.Users.AnyAsync(user => user.Email == AdminEmail);
        if (adminExists)
        {
            return;
        }

        var passwordHash = PasswordHasher.HashPassword(AdminPassword, secretKey);
        var admin = new User(AdminName, AdminEmail, passwordHash, Role.Admin);

        await context.Users.AddAsync(admin);
        await context.SaveChangesAsync();
    }
}
