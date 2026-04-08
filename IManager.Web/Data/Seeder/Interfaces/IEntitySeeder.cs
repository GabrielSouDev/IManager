namespace IManager.Web.Data.Seeder.Interfaces;

internal interface IEntitySeeder<in T>
{
    Task SeedAsync(IEnumerable<T> data);
}
