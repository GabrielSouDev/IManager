namespace IManager.Web.Presentation.ViewModels.Companies;

public class CompanyViewModel
{
    public Guid Id { get; set; }
    public string DocumentNumber { get; set; } = string.Empty;
    public string LegalName { get; set; } = string.Empty;
    public string TradeName { get; set; } = string.Empty;
    public DateOnly FoundedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }
}
