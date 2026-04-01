using IManager.Web.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;
namespace IManager.Web.Shared.DTO.TimeTrackings;

public class TimeEntryDTO
{
    public Guid Id { get; set; }
    public IEnumerable<TimeCheckDTO> Checks { get; set; } = new List<TimeCheckDTO>();
    public TimeEntryStatus Status { get; set; }
    public TimeSpan HoursWorked { get; set; }
}
