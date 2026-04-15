using IManager.Web.Presentation.ViewModels.Companies;
using IManager.Web.Presentation.ViewModels.JobTitles;

namespace IManager.Web.Presentation.ViewModels.Departments;

public class DepartmentViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public CompanyViewModel Company { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }
}
