using IManager.Web.Presentation.Configurations;

namespace IManager.Web.Presentation.Extensions;

public static class ConfigurationExtensions
{
    public static void AddConfigurations(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("Email"));
    }
}