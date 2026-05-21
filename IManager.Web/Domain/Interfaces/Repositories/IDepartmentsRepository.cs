using IManager.Web.Domain.Entities.Companies;
using IManager.Web.Presentation.ViewModels.Departments;

namespace IManager.Web.Domain.Interfaces.Repositories
{
    public interface IDepartmentsRepository : IRepository<Department>
    {
        Task<InfoDepartmentViewModel?> GetInfoByIdAsync(Guid id);
    }
}