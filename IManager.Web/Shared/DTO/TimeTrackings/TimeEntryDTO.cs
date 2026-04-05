using IManager.Web.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;
namespace IManager.Web.Shared.DTO.TimeTrackings;

public class TimeEntryDTO
{
    public Guid Id { get; set; }
    public DateOnly Date { get; set; }
    public List<TimeCheckDTO> Checks { get; set; } = new List<TimeCheckDTO>();
    public bool IsConsistent { get; set; }
    public TimeEntryStatus Status { get; set; }
    public TimeSpan HoursWorked { get; set; }
    public string? AdjustmentReason { get; set; }
    public string? RejectionReason { get; set; }
}