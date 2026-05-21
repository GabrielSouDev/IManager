namespace IManager.Web.Presentation.ViewModels.Departments;

public class InfoDepartmentViewModel
{
    public int EmployeeCount { get; set; }
    public decimal AverageSalary { get; set; }
    public string MostCommonJobTitle { get; set; } = string.Empty;
    public decimal HighestCostJobTitle { get; set; }
}