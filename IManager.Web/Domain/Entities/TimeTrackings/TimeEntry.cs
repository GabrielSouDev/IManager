using IManager.Web.Domain.Entities.Users;

namespace IManager.Web.Domain.Entities.TimeTrackings;

public class TimeEntry : BaseEntity
{
    public Guid EmployeeId { get; set; }
    public UserProfile Employee { get; set; } = null!;

    public ICollection<TimeCheck> Checks { get; set; } = new List<TimeCheck>();

    public decimal HoursWorked { get; set; }
}
