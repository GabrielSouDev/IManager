using IManager.Web.Presentation.ViewModels.Departments;
using IManager.Web.Shared;

namespace IManager.Web.Application.Interfaces;

public interface IDepartmentService
{
    Task<DepartmentViewModel?> GetViewModelByIdAsync(Guid id);
    Task<IEnumerable<DepartmentHierarchyViewModel>> GetDepartmentsHierarchyViewModelAsync(Guid? companyId = null);
    Task<PagedResult<DepartmentViewModel>> GetPagedAsync(string search, ActiveFilter active, int page, int pageSize, Guid? companyId = null);
    Task<Result> AddAsync(CreateDepartmentViewModel department);
    Task<EditDepartmentViewModel> GetEditViewModelByIdAsync(Guid value);
    Task<Result> UpdateAsync(EditDepartmentViewModel department);
    Task<Result> SoftDeleteAsync(Guid id);
}