using IManager.Web.Data.Persistence;
using IManager.Web.Domain.Entities.Companies;
using IManager.Web.Domain.Interfaces.Repositories;
using IManager.Web.Presentation.ViewModels.JobTitles;
using Microsoft.EntityFrameworkCore;

namespace IManager.Web.Data.Repositories;

public class JobTitlesRepository : Repository<JobTitle>, IJobTitlesRepository
{
    public JobTitlesRepository(ApplicationDbContext context) : base(context) { }

    public async Task<InfoJobTitleViewModel?> GetInfoByIdAsync(Guid id)
    {
        var start = new DateTime(DateTime.UtcNow.Year, 1, 1, 0, 0, 0, kind: DateTimeKind.Utc);
        var end = start.AddYears(1);

        var result = await _dbSet
            .Where(j => j.Id == id)
            .Select(j => new InfoJobTitleViewModel
            {
                EmployeeCount = j.Employees.Count(),

                AverageSalary = j.Employees
                    .Where(e => e.Payslips
                        .Any(p => p.CreatedAt >= start && p.CreatedAt < end))
                        .Average(e => e.Payslips.Where(p => p.CreatedAt >= start && p.CreatedAt < end)
                        .Average(p => p.NetSalary)),

                TotalCost = j.Employees
                    .SelectMany(e => e.Payslips)
                    .Where(p => p.CreatedAt >= start && p.CreatedAt < end)
                    .Sum(p => (decimal?)p.GrossSalary) ?? 0
            })
            .FirstOrDefaultAsync();

        return result;
    }
}
