using IManager.Web.Data.Persistence;
using IManager.Web.Domain.Entities.Users;
using IManager.Web.Presentation.ViewModels.Account;
using Microsoft.AspNetCore.Identity;

namespace IManager.Web.Presentation.Extensions;

public static class InitializeExtensions
{
    public static async Task Initialize(this WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

            var adminRequest = app.Configuration.GetSection("AdminUser").Get<RegisterViewModel>()
                ?? throw new Exception("AdminUser config not found");

            await db.SeedDataAsync(userManager, roleManager, adminRequest);
        }
    }
}