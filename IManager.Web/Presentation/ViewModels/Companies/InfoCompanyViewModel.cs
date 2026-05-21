namespace IManager.Web.Presentation.ViewModels.Companies;

public class InfoCompanyViewModel
{
    public int EmployeeCount { get; set; }
    public int DepartmentCount { get; set; }
    public decimal AverageSalary { get; set; }
    public decimal AnnualPayrollCost { get; set; }
    public string LastCreatedUser { get; set; } = string.Empty;
    public DateTime LastCreatedDate { get; set; }
}