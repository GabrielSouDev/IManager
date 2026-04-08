using IManager.Web.Data.Seeder.SeedDatas;
using IManager.Web.Domain.Consts;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace IManager.Web.Data.Seeder.Builders;

public static class DemoSeedBuilder
{
    private static readonly HashSet<string> UsedCompanyNames = new();
    private static readonly HashSet<string> UsedEmails = new();

    public static DemoSeedData Build()
    {
        string fantasyName;
        do
        {
            fantasyName = GenerateCompanyName();
        }
        while (!UsedCompanyNames.Add(NormalizeKey(fantasyName)));

        var companyId = Guid.NewGuid();

        var company = new CompanySeedData(
            companyId,
            GenerateCnpj(),
            $"{fantasyName} LTDA",
            fantasyName,
            RandomFoundedDate()
        );

        var departments = BuildDepartments(companyId);
        var jobTitles = BuildJobTitles(departments);
        var users = BuildUsers(companyId, jobTitles, fantasyName);

        return new DemoSeedData
        {
            Company = company,
            Departments = departments,
            JobTitles = jobTitles,
            Users = users
        };
    }

    private static List<DepartmentSeedData> BuildDepartments(Guid companyId)
        => new[]
        {
            "Tecnologia","Financeiro","RH","Comercial","Operações","Marketing"
        }
        .Select(n => new DepartmentSeedData(Guid.NewGuid(), n, companyId))
        .ToList();

    private static List<JobTitleSeedData> BuildJobTitles(IEnumerable<DepartmentSeedData> departments)
    {
        var list = new List<JobTitleSeedData>();
        foreach (var d in departments)
        {
            list.Add(new JobTitleSeedData(Guid.NewGuid(), $"Analista de {d.Name}", d.Id));
            list.Add(new JobTitleSeedData(Guid.NewGuid(), $"Especialista em {d.Name}", d.Id));
        }
        return list;
    }

    private static List<UserSeedData> BuildUsers(
        Guid companyId,
        IReadOnlyList<JobTitleSeedData> jobTitles,
        string fantasyName)
    {
        var firstNames = new[]
        {
            "Gabriel","Ana","Lucas","Mariana","Carlos","Beatriz","Felipe","Juliana",
            "Rafael","Camila","Bruno","Laura","Thiago","Renata","Eduardo","Patricia",
            "Diego","Fernanda","Rodrigo","Aline"
        };

        var lastNames = new[]
        {
            "Souza","Silva","Oliveira","Pereira","Lima","Gomes","Ribeiro","Almeida",
            "Costa","Araújo","Rocha","Martins","Barbosa","Ferreira","Teixeira","Freitas"
        };

        var users = new List<UserSeedData>();

        var domain = NormalizeKey(fantasyName);
        var adminEmail = GenerateUniqueEmail($"admin@{domain}.com");

        users.Add(new UserSeedData(
            Guid.NewGuid(),
            adminEmail,
            "Admin@123",
            Role.Admin,
            $"{fantasyName} Administrator",
            GenerateCpf(),
            new DateOnly(2000, 1, 1),
            companyId,
            jobTitles[0].Id,
            0m
        ));

        for (int i = 0; i < 20; i++)
        {
            var first = firstNames[RandomNumberGenerator.GetInt32(firstNames.Length)];
            var last = lastNames[RandomNumberGenerator.GetInt32(lastNames.Length)];
            var job = jobTitles[RandomNumberGenerator.GetInt32(jobTitles.Count)];

            var baseEmail = RemoveAccents($"{first.ToLower()}.{last.ToLower()}@demo.com");
            var email = GenerateUniqueEmail(baseEmail);

            users.Add(new UserSeedData(
                Guid.NewGuid(),
                email,
                GeneratePassword(),
                Role.User,
                $"{first} {last}",
                GenerateCpf(),
                RandomBirthDate(),
                companyId,
                job.Id,
                GenerateBaseSalary()
            ));
        }

        return users;
    }

    private static string GenerateUniqueEmail(string baseEmail)
    {
        var email = baseEmail;
        var i = 1;

        while (!UsedEmails.Add(email))
        {
            var parts = baseEmail.Split('@');
            email = $"{parts[0]}{i}@{parts[1]}";
            i++;
        }

        return email;
    }

    private static string GenerateCompanyName()
    {
        var prefixes = new[] { "Nova", "Global", "Alpha", "Prime", "Tech", "Digital", "Smart", "Next" };
        var cores = new[] { "Solution", "Systems", "Soft", "Tecnologia", "Consultoria", "Services", "Group", "Labs" };
        return $"{prefixes[RandomNumberGenerator.GetInt32(prefixes.Length)]} {cores[RandomNumberGenerator.GetInt32(cores.Length)]}";
    }

    private static string GeneratePassword(int length = 12)
    {
        if (length < 6)
            throw new ArgumentException("Password length must be at least 6.");

        var upper = "ABCDEFGHJKLMNPQRSTUVWXYZ";
        var lower = "abcdefghijkmnopqrstuvwxyz";
        var digits = "0123456789";
        var symbols = "@#!?";

        var all = upper + lower + digits + symbols;

        var chars = new List<char>
    {
        upper[RandomNumberGenerator.GetInt32(upper.Length)],
        lower[RandomNumberGenerator.GetInt32(lower.Length)],
        digits[RandomNumberGenerator.GetInt32(digits.Length)],
        symbols[RandomNumberGenerator.GetInt32(symbols.Length)]
    };

        for (int i = chars.Count; i < length; i++)
            chars.Add(all[RandomNumberGenerator.GetInt32(all.Length)]);

        // embaralha para não ficar previsível
        return new string(
            chars.OrderBy(_ => RandomNumberGenerator.GetInt32(int.MaxValue)).ToArray()
        );
    }

    private static decimal GenerateBaseSalary()
        => RandomNumberGenerator.GetInt32(2000_00, 4000_01) / 100m;

    private static DateOnly RandomBirthDate()
        => new DateOnly(1980, 1, 1).AddDays(RandomNumberGenerator.GetInt32(0, 9000));

    private static DateOnly RandomFoundedDate()
        => new DateOnly(RandomNumberGenerator.GetInt32(1990, 2021), RandomNumberGenerator.GetInt32(1, 13), 1);

    private static string GenerateCpf()
        => string.Concat(Enumerable.Range(0, 11).Select(_ => RandomNumberGenerator.GetInt32(0, 10)));

    private static string GenerateCnpj()
        => string.Concat(Enumerable.Range(0, 14).Select(_ => RandomNumberGenerator.GetInt32(0, 10)));

    private static string NormalizeKey(string text)
        => RemoveAccents(text).Replace(" ", "").ToLowerInvariant();

    private static string RemoveAccents(string text)
    {
        var normalized = text.Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder();
        foreach (var c in normalized)
            if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                sb.Append(c);
        return sb.ToString().Normalize(NormalizationForm.FormC);
    }
}