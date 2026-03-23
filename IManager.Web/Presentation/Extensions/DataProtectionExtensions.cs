using Microsoft.AspNetCore.DataProtection;

namespace IManager.Web.Presentation.Extensions;

public static class DataProtectionExtensions
{
    public static void AddDataProtectionConfig(
    this WebApplicationBuilder builder)
    {
        builder.Services.AddDataProtection()
            .PersistKeysToFileSystem(new DirectoryInfo("/app/keys"))
            .SetApplicationName("IManager");
    }
}