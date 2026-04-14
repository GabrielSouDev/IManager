using IManager.Web.Presentation.ViewModels.Companies;
using IManager.Web.Shared;

namespace IManager.Web.Application.Interfaces;

public interface ICompanyService
{
    Task<Result> AddAsync(CreateCompanyViewModel company);
    Task<Result> SoftDeleteAsync(Guid id);
    Task<IEnumerable<CompanyHierarchyViewModel>> GetCompaniesHierarchyViewModelAsync();
    Task<EditCompanyViewModel?> GetEditViewModelByIdAsync(Guid value);
    Task<PagedResult<CompanyViewModel>> GetPagedAsync(string search, ActiveFilter active, int page, int pageSize);
    Task<CompanyViewModel> GetViewModelByIdAsync(Guid value);
    Task<Result> UpdateAsync(Guid id, EditCompanyViewModel company);
}