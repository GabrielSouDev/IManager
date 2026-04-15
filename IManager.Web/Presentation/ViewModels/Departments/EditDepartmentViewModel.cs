using IManager.Web.Presentation.ViewModels.Companies;
using IManager.Web.Presentation.ViewModels.JobTitles;

namespace IManager.Web.Presentation.ViewModels.Departments;

public class EditDepartmentViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
