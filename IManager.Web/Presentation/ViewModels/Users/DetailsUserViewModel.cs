namespace IManager.Web.Presentation.ViewModels.Users;

public class DetailsUserViewModel
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public InfoUserProfileViewModel Info { get; set; } = new();
    public string DocumentNumber { get; set; } = string.Empty;
    public DateOnly BirthDate { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public decimal BaseSalary { get; set; } = 0m;
    public bool IsActive { get; set; }
}