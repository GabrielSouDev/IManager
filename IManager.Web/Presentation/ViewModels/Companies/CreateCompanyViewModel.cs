namespace IManager.Web.Presentation.ViewModels.Companies;

public class CreateCompanyViewModel
{
    public string DocumentNumber { get; set; } = string.Empty;
    public string LegalName { get; set; } = string.Empty;
    public string TradeName { get; set; } = string.Empty;
    public DateOnly FoundedAt { get; set; }
}
