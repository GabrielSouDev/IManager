using IManager.Web.Presentation.ViewModels.Companies;
using IManager.Web.Presentation.ViewModels.JobTitles;

namespace IManager.Web.Presentation.ViewModels.Account;

public class EditAccountViewModel
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public Guid DepartmentId { get; set; }
    public Guid JobTitleId { get; set; } 
    public string FullName { get; set; } = string.Empty;
    public string DocumentNumber { get; set; } = string.Empty;
    public DateOnly BirthDate { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public decimal BaseSalary { get; set; } = 0m;
}