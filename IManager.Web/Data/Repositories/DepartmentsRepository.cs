using IManager.Web.Data.Persistence;
using IManager.Web.Domain.Entities.Companies;
using IManager.Web.Domain.Interfaces.Repositories;
using IManager.Web.Presentation.ViewModels.Departments;
using Microsoft.EntityFrameworkCore;

namespace IManager.Web.Data.Repositories;

public class DepartmentsRepository : Repository<Department>, IDepartmentsRepository
{
    public DepartmentsRepository(ApplicationDbContext context) : base(context) { }

    public async Task<InfoDepartmentViewModel?> GetInfoByIdAsync(Guid id)
    {
        var exists = await ExistsAsync(d => d.Id == id);
        if (!exists) return null;

        var result = await _dbSet.Where(d => d.Id == id).Select(d => new InfoDepartmentViewModel
        {
            AverageSalary = d.JobTitles
                .SelectMany(j => j.Employees)
                .SelectMany(e => e.Payslips)
                .Average(p => p.NetSalary),

            EmployeeCount = d.JobTitles.Sum(j => j.Employees.Count),

            HighestCostJobTitle = d.JobTitles
                .GroupBy(j=>j.Name)
                .Max(g=>g.SelectMany(j=>j.Employees)
                .SelectMany(e=>e.Payslips)
                .Sum(p=>p.GrossSalary)),

            MostCommonJobTitle = d.JobTitles.GroupBy(j => j.Name)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .FirstOrDefault() ?? string.Empty,

            JobTitleCount = d.JobTitles.Count(),
        }).FirstOrDefaultAsync();

        return result;
    }
}
