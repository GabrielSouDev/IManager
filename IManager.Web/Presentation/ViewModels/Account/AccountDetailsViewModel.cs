using IManager.Web.Presentation.ViewModels.JobTitles;

namespace IManager.Web.Presentation.ViewModels.Account;

public class AccountDetailsViewModel
{
    public string FullName { get; set; } = string.Empty;
    public string DocumentNumber { get; set; } = string.Empty;
    public DateOnly BirthDate { get; set; }
    public string? PhoneNumber { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public JobTitleModelView? JobTitle { get; set; }
    public decimal BaseSalary { get; set; } = 0m;
}