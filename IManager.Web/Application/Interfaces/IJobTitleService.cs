using IManager.Web.Presentation.ViewModels.Departments;
using IManager.Web.Presentation.ViewModels.JobTitles;
using IManager.Web.Shared;

namespace IManager.Web.Application.Interfaces;

public interface IJobTitleService
{
    Task<Result> AddJobTitle(CreateJobTitleModelView model);
    Task<DetailsJobTitleModelView> GetDetailsModelView(Guid id);
    Task<PagedResult<IndexJobTitleModelView>> GetPagedAsync(string search, ActiveFilter active, int page, int pageSize, Guid? companyId = null);
}