using Serilog;
using Serilog.Events;

namespace IManager.Web.Presentation.Extensions;

public static class LoggingExtensions
{
    public static void AddLogging(this WebApplicationBuilder builder)
    {
        var template = "[{Timestamp:HH:mm:ss} {Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}";
        var logsPath = Path.Combine(Directory.GetCurrentDirectory(), "Logs", "log-.txt");

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .WriteTo.Console(outputTemplate: template)
            .WriteTo.File(
                path: logsPath,
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 30,
                outputTemplate: template
            )
            .CreateLogger();

        builder.Host.UseSerilog();
    }
}