using IManager.Web.Data.Persistence;
using IManager.Web.Domain.Interfaces.Persistence;
using IManager.Web.Presentation.Configurations;
using Microsoft.EntityFrameworkCore;

namespace IManager.Web.Presentation.Extensions;

public static class ContextExtensions
{
    public static void AddContext(this WebApplicationBuilder builder)
    {
        builder.Services.AddHttpContextAccessor();

        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

        var postgresSettings = builder.Configuration.GetSection("Postgres").Get<PostgresSettings>() ??
                                        throw new Exception("Não foi possivel configurar o Postgres.");

        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(postgresSettings.ConnectionString, o =>
                o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery))
        );
    }
}