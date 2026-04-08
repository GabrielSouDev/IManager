using IManager.Web.Data.Seeder.SeedDatas;
using IManager.Web.Domain.Consts;
using IManager.Web.Presentation.Configurations;

namespace IManager.Web.Data.Seeder.Builders;

public static class DemoFixedSeedBuilder
{
    public static DemoSeedData Build(DemoProfilesOptions options)
    {
        var companyId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");

        var company = new CompanySeedData(
            companyId,
            options.Company.DocumentNumber,
            $"{options.Company.TradeName} LTDA",
            options.Company.TradeName,
            new DateOnly(2020, 1, 1)
        );

        var department = new DepartmentSeedData(Guid.NewGuid(), "Staff", companyId);
        var jobTitle = new JobTitleSeedData(Guid.NewGuid(), "Staff", department.Id);

        var users = new[]
        {
            CreateUser(options.Users.Admin, Role.Admin, companyId, jobTitle.Id),
            CreateUser(options.Users.Staff, Role.Staff, companyId, jobTitle.Id),
            CreateUser(options.Users.User, Role.User, companyId, jobTitle.Id)
        };

        return new DemoSeedData
        {
            Company = company,
            Departments = new[] { department },
            JobTitles = new[] { jobTitle },
            Users = users
        };
    }

    private static UserSeedData CreateUser(
        DemoUserOptions options,
        string role,
        Guid companyId,
        Guid jobTitleId)
        => new(
            Guid.NewGuid(),
            options.Email,
            options.Password,
            role,
            options.FullName,
            options.DocumentNumber,
            new DateOnly(1995, 1, 1),
            companyId,
            jobTitleId,
            0m
        );
}