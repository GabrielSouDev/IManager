using IManager.Web.Presentation.ViewModels.Departments;
using IManager.Web.Presentation.ViewModels.JobTitles;
using IManager.Web.Shared;

namespace IManager.Web.Application.Interfaces;

public interface IJobTitleService
{
    Task<Result> CreateAsync(CreateJobTitleModelView model);
    Task<Result> SoftDeleteAsync(Guid id);
    Task<DetailsJobTitleModelView?> GetDetailsModelViewById(Guid id);
    Task<EditJobTitleModelView?> GetEditModelViewByIdAsync(Guid id);
    Task<JobTitleModelView?> GetModelViewByIdAsync(Guid id);
    Task<PagedResult<IndexJobTitleModelView>> GetPagedAsync(string search, ActiveFilter active, int page, int pageSize, Guid? companyId = null);
    Task<Result> UpdateAsync(EditJobTitleModelView model);
}