using IManager.Web.Presentation.ViewModels.Companies;

namespace IManager.Web.Application.Interfaces;

public interface ICompanyService
{
    Task<CompanyViewModel> GetByIdAsync(Guid? id);
    Task<IEnumerable<CompanyHierarchyViewModel>> GetCompaniesHierarchyViewModelAsync();
    Task<PagedResult<CompanyViewModel>> GetPagedAsync(int page, int pageSize, string search);
}