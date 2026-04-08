using IManager.Web.Data.Persistence;
using IManager.Web.Data.Seeder.Builders;
using IManager.Web.Data.Seeder.Interfaces;
using IManager.Web.Domain.Consts;
using IManager.Web.Domain.Entities.Users;
using IManager.Web.Presentation.Configurations;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Serilog;

namespace IManager.Web.Presentation.Extensions;

public static class InitializeExtensions
{
    public static async Task Initialize(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
        var demoSeeder = scope.ServiceProvider.GetRequiredService<IDemoSeeder>();
        var demoOptions = scope.ServiceProvider.GetRequiredService<IOptions<DemoProfilesOptions>>().Value;

        var retries = 1;
        while (true)
        {
            try
            {
                await dbContext.Database.MigrateAsync();
                break;
            }
            catch (Npgsql.NpgsqlException ex)
            {
                Console.WriteLine($"# {retries} - Falha ao conectar no Postgres: {ex.Message}");
                retries++;
                if (retries >= 10) throw;
                await Task.Delay(2000);
            }
        }

        foreach (var role in Role.All)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole<Guid>(role));
        }

        if (userManager.Users.Any()) return;

        Log.Information("Iniciando seed DEMO...");

        await demoSeeder.SeedAsync(DemoFixedSeedBuilder.Build(demoOptions));

        for (int i = 0; i < 3; i++)
            await demoSeeder.SeedAsync(DemoSeedBuilder.Build());
    }
}