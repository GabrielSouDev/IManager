namespace IManager.Web.Presentation.ViewModels.Users;

public class InfoUserProfileViewModel
{
    public string CompanyDocumentNumber { get; set; } = string.Empty;
    public string CompanyTradeName { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string JobTitle { get; set; } = string.Empty;
    public decimal LastAnnualNetSalary { get; set; }
    public decimal LastAnnualGrossSalary { get; set; }
    public decimal AverageNetSalary { get; set; }
    public decimal AverageGrossSalary { get; set; }

}