using IManager.Web.Presentation.ViewModels.Departments;

namespace IManager.Web.Presentation.ViewModels.Companies;

public class CompanyHierarchyViewModel
{
    public Guid Id { get; set; }
    public string DocumentNumber { get; set; } = string.Empty;
    public string LegalName { get; set; } = string.Empty;
    public string TradeName { get; set; } = string.Empty;
    public IEnumerable<DepartmentHierarchyViewModel> Departments { get; set; } = new List<DepartmentHierarchyViewModel>();
    public DateOnly FoundedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}
