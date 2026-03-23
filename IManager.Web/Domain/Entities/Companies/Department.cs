namespace IManager.Web.Domain.Entities.Companies;

public class Department : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public Guid CompanyId { get; set; }
    public Company Company { get; set; } = null!;

    public ICollection<JobTitle> JobTitles { get; set; } = new List<JobTitle>();
}
