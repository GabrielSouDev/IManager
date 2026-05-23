using IManager.Web.Presentation.ViewModels.Companies;
using IManager.Web.Shared;
using System.Collections;

namespace IManager.Web.Application.Interfaces;

public interface ICompanyService
{
    Task<Result> CreateAsync(CreateCompanyViewModel company);
    Task<Result> SoftDeleteAsync(Guid id);
    Task<IEnumerable<CompanyHierarchyViewModel>> GetCompaniesHierarchyViewModelAsync();
    Task<EditCompanyViewModel?> GetEditViewModelByIdAsync(Guid id);
    Task<PagedResult<IndexCompanyViewModel>> GetPagedAsync(string search, ActiveFilter active, int page, int pageSize);
    Task<CompanyViewModel?> GetViewModelByIdAsync(Guid id);
    Task<DetailsCompanyViewModel?> GetDetailsViewModelByIdAsync(Guid id);
    Task<Result> UpdateAsync(Guid id, EditCompanyViewModel company);
    Task<IEnumerable<CompanyViewModel>> GetCompaniesViewModelsAsync();
}