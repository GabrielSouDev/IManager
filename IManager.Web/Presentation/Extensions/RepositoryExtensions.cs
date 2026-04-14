using IManager.Web.Data.Repositories;
using IManager.Web.Domain.Interfaces.Repositories;

namespace IManager.Web.Presentation.Extensions;

public static class RepositoryExtensions
{
    public static void AddRepositories(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
    }
}