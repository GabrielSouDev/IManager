using IManager.Web.Domain.Entities.Companies;
using IManager.Web.Presentation.ViewModels.Companies;

namespace IManager.Web.Domain.Interfaces.Repositories
{
    public interface ICompaniesRepository : IRepository<Company>
    {
        Task<InfoCompanyViewModel?> GetInfoByIdAsync(Guid id);
    }
}