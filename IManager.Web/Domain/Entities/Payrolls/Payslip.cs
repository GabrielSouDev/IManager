using IManager.Web.Domain.Entities.Users;

namespace IManager.Web.Domain.Entities.Payrolls;

public class Payslip : BaseEntity
{
    public Guid PayrollId { get; set; }
    public Payroll Payroll { get; set; } = null!;

    public Guid EmployeeId { get; set; }
    public UserProfile Employee { get; set; } = null!;

    public decimal GrossSalary { get; set; } = 0m;
    public decimal TotalEarnings => GrossSalary
                                    + OvertimeAdditionals
                                    + HazardPay
                                    + UnhealthyPay
                                    + Commission;
    public decimal TotalDeductions => INSSDeduction + IRRFDeduction + OtherDeductions;
    public decimal NetSalary => TotalEarnings - TotalDeductions;

    public decimal OvertimeAdditionals { get; set; } = 0m;
    public decimal HazardPay { get; set; } = 0m;
    public decimal UnhealthyPay { get; set; } = 0m;
    public decimal Commission { get; set; } = 0m;
    public decimal INSSDeduction { get; set; } = 0m;
    public decimal IRRFDeduction { get; set; } = 0m;
    public decimal OtherDeductions { get; set; } = 0m;
}