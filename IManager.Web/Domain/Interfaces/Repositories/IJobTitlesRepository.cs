using IManager.Web.Domain.Entities.Companies;
using IManager.Web.Presentation.ViewModels.JobTitles;

namespace IManager.Web.Domain.Interfaces.Repositories
{
    public interface IJobTitlesRepository : IRepository<JobTitle>
    {
        Task<InfoJobTitleViewModel?> GetInfoByIdAsync(Guid id);
    }
}