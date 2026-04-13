using IManager.Web.Presentation.ViewModels.Companies;
using IManager.Web.Shared;

namespace IManager.Web.Application.Interfaces;

public interface ICompanyService
{
    Task<Result> AddAsync(CreateCompanyViewModel company);
    Task<CompanyViewModel> GetByIdAsync(Guid? id);
    Task<IEnumerable<CompanyHierarchyViewModel>> GetCompaniesHierarchyViewModelAsync();
    Task<PagedResult<CompanyViewModel>> GetPagedAsync(int page, int pageSize, string search);
}