using IManager.Web.Presentation.ViewModels.Companies;

namespace IManager.Web.Application.Interfaces;

public interface ICompanyService
{
    Task<IEnumerable<CompanyHierarchyViewModel>> GetCompaniesHierarchyViewModelAsync();
}