namespace IManager.Web.Data.Seeder.SeedDatas;

public record CompanySeedData(
    Guid Id,
    string DocumentNumber,
    string LegalName,
    string TradeName,
    DateOnly FoundedAt
);
