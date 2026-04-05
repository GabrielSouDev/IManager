using IManager.Web.Domain.Entities.Users;
using IManager.Web.Domain.Enums;
using NuGet.Packaging.Signing;
using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;
using System.Numerics;

namespace IManager.Web.Domain.Entities.TimeTrackings;

public class TimeEntry : BaseEntity
{
    public Guid EmployeeId { get; set; }
    public UserProfile Employee { get; set; } = null!;

    public ICollection<TimeCheck> Checks { get; set; } = new List<TimeCheck>();

    public DateOnly Date { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
    public TimeEntryStatus Status { get; set; } = TimeEntryStatus.Accepted;
    public bool IsCurrent { get; set; } = true;
    public Guid? ParentId { get; set; }
    public string? AdjustmentReason { get; set; }
    public string? RejectionReason { get; set; }

    [NotMapped]
    public bool IsConsistent => Checks.Count % 2 == 0 ? true : false; 

    [NotMapped]
    public TimeSpan HoursWorked => GetHoursWorked(); 

    private TimeSpan GetHoursWorked()
    {
        var totalHours = TimeSpan.Zero;
        if (Checks.Count <= 1)
            return totalHours;

        var timestamps = Checks.Select(c => c.Timestamp.TimeOfDay).OrderBy(ts => ts);
        var checks = new Stack<TimeSpan>(timestamps);

        if(!IsConsistent)
            checks.Pop();

        for (int i = checks.Count; i > 0; i -= 2)
        {
            totalHours += checks.Pop() - checks.Pop();
        }

        return totalHours;
    }
}
