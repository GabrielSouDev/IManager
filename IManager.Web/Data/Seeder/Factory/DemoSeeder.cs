using IManager.Web.Data.Seeder.Interfaces;
using IManager.Web.Data.Seeder.SeedDatas;
using IManager.Web.Domain.Interfaces.Persistence;

namespace IManager.Web.Data.Seeder.Factory;

public sealed class DemoSeeder : IDemoSeeder
{
    private readonly CompanySeeder _company;
    private readonly DepartmentSeeder _departments;
    private readonly JobTitleSeeder _jobTitles;
    private readonly UserSeeder _users;
    private readonly IUnitOfWork _uow;

    public DemoSeeder(
        CompanySeeder company,
        DepartmentSeeder departments,
        JobTitleSeeder jobTitles,
        UserSeeder users,
        IUnitOfWork uow)
    {
        _company = company;
        _departments = departments;
        _jobTitles = jobTitles;
        _users = users;
        _uow = uow;
    }

    public async Task SeedAsync(DemoSeedData data)
    {
        await _uow.BeginTransactionAsync();
        try
        {
            await _company.SeedAsync(data.Company);
            await _departments.SeedAsync(data.Departments);
            await _jobTitles.SeedAsync(data.JobTitles);
            await _users.SeedAsync(data.Users);

            await _uow.CommitAsync();
        }
        catch
        {
            await _uow.RollbackAsync();
            throw;
        }
    }
}