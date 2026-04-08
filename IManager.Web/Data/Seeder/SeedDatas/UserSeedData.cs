namespace IManager.Web.Data.Seeder.SeedDatas;

public record UserSeedData(
    Guid Id,
    string Email,
    string Password,
    string Role,
    string FullName,
    string DocumentNumber,
    DateOnly BirthDate,
    Guid CompanyId,
    Guid JobTitleId,
    decimal BaseSalary
);