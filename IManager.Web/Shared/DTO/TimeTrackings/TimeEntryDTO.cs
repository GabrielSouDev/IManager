using IManager.Web.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;
namespace IManager.Web.Shared.DTO.TimeTrackings;

public class TimeEntryDTO
{
    public IEnumerable<TimeCheckDTO> Checks { get; set; } = new List<TimeCheckDTO>();
    public TimeEntryStatus Status;
    public TimeSpan HoursWorked;
}
