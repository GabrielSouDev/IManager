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

    [IntegerValidator(MinValue = 1, MaxValue = 12)]
    public int Month { get; set; }

    [IntegerValidator(MinValue = 2026)]
    public int Year { get; set; }

    [NotMapped]
    public TimeEntryStatus Status => Checks.Count % 2 == 0 ? TimeEntryStatus.Valid : TimeEntryStatus.Inconsistent;

    [NotMapped]
    public TimeSpan HoursWorked => GetHoursWorked(); 

    private TimeSpan GetHoursWorked()
    {
        var totalHours = TimeSpan.Zero;
        if (Checks.Count == 0)
            return totalHours;

        var timestamps = Checks.Select(c => c.Timestamp.TimeOfDay).OrderBy(ts => ts);
        var checks = new Stack<TimeSpan>(timestamps);

        if(Status == TimeEntryStatus.Inconsistent)
            checks.Pop();

        for (int i = checks.Count; i > 0; i -= 2)
        {
            totalHours += checks.Pop() - checks.Pop();
        }

        return totalHours;
    }
}
