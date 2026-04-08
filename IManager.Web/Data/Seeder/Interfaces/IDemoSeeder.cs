using IManager.Web.Data.Seeder.SeedDatas;

namespace IManager.Web.Data.Seeder.Interfaces;

public interface IDemoSeeder
{
    Task SeedAsync(DemoSeedData data);
}
