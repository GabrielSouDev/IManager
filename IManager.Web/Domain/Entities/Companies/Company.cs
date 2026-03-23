using IManager.Web.Domain.Entities.Payrolls;
using IManager.Web.Domain.Entities.Users;

namespace IManager.Web.Domain.Entities.Companies;

public class Company : BaseEntity
{
    public string DocumentNumber { get; set; } = string.Empty;
    public string LegalName { get; set; } = string.Empty;
    public string TradeName { get; set; } = string.Empty;
    public DateOnly FoundedAt { get; set; }
    public ICollection<UserProfile> Employees { get; set; } = new List<UserProfile>();
    public ICollection<Department> Departments { get; set; } = new List<Department>();
    public ICollection<Payroll> Payrolls { get; set; } = new List<Payroll>();
}
