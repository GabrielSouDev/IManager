using IManager.Web.Data.Persistence;
using IManager.Web.Domain.Entities.Companies;
using IManager.Web.Domain.Interfaces.Repositories;
using IManager.Web.Presentation.ViewModels.Departments;

namespace IManager.Web.Data.Repositories;

public class DepartmentsRepository : Repository<Department>, IDepartmentsRepository
{
    public DepartmentsRepository(ApplicationDbContext context) : base(context) { }

    public async Task<InfoDepartmentViewModel?> GetInfoByIdAsync(Guid id)
    {
        var exists = await ExistsAsync(id);
        if (!exists) return null;

        var result = _dbSet.Where(d => d.Id == id).Select(d => new InfoDepartmentViewModel
        {
            AverageSalary = d.JobTitles.SelectMany(j => j.Employees)
                                       .SelectMany(e => e.Payslips)
                                       .Average(p => p.NetSalary),

            EmployeeCount = d.JobTitles.Sum(j => j.Employees.Count),
            HighestCostJobTitle = 0,
            MostCommonJobTitle = ""
        }).FirstOrDefault();

        return result;
    }
}
