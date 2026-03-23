using Microsoft.AspNetCore.RateLimiting;

namespace IManager.Web.Presentation.Extensions;

public static class RateLimitExtensions
{
    public static void AddRateLimitConfig(
    this WebApplicationBuilder builder)
    {
        builder.Services.AddRateLimiter(options =>
        {
            options.AddFixedWindowLimiter("login", config =>
            {
                config.PermitLimit = 10;
                config.Window = TimeSpan.FromMinutes(1);
                config.QueueLimit = 0;
            });
        });
    }
}
