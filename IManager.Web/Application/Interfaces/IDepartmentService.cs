using IManager.Web.Presentation.ViewModels.Departments;

namespace IManager.Web.Application.Interfaces;

public interface IDepartmentService
{
    Task<IEnumerable<DepartmentHierarchyViewModel>> GetDepartmentsHierarchyViewModelAsync(Guid? companyId = null);
}