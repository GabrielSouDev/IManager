using IManager.Web.Domain.Entities.Companies;

namespace IManager.Web.Domain.Entities.Payrolls;

public class Payroll : BaseEntity
{
    public Guid CompanyId { get; set; }
    public Company Company { get; set; } = null!;

    public DateOnly PeriodStart { get; set; }
    public DateOnly PeriodEnd { get; set; }

    public ICollection<Payslip> Payslips { get; set; } = new List<Payslip>();
}
