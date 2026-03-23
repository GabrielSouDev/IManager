using IManager.Web.Domain.Consts;
using IManager.Web.Domain.Entities.Companies;
using IManager.Web.Domain.Entities.Payrolls;
using IManager.Web.Domain.Entities.TimeTrackings;

namespace IManager.Web.Domain.Entities.Users;

public class UserProfile : BaseEntity
{
    public string FullName { get; set; } = string.Empty;
    public string DocumentNumber { get; set; } = string.Empty;
    public DateOnly BirthDate { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime? DeletedAt { get; set; }
    public string Role { get; set; } = string.Empty;

    public Guid CompanyId { get; set; }
    public Company Company { get; set; } = null!;

    public Guid JobTitleId { get; set; }
    public JobTitle JobTitle { get; set; } = null!;

    public decimal BaseSalary { get; set; } = 0m;

    public ICollection<TimeEntry> TimeEntries { get; set; } = new List<TimeEntry>();
    public ICollection<Payslip> Payslips { get; set; } = new List<Payslip>();


    public void Deactivate()
    {
        IsActive = false;
        DeletedAt = DateTime.UtcNow;
    }
}
