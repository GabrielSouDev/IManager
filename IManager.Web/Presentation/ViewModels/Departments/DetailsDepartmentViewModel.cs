using IManager.Web.Presentation.ViewModels.Companies;

namespace IManager.Web.Presentation.ViewModels.Departments;

public class DetailsDepartmentViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public InfoDepartmentViewModel Info { get; set; } = new();
    public CompanyViewModel Company { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }
}
