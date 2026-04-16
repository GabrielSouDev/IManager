using IManager.Web.Presentation.ViewModels.Departments;
using IManager.Web.Presentation.ViewModels.JobTitles;

namespace IManager.Web.Application.Interfaces;

public interface IJobTitleService
{
    Task<PagedResult<IndexJobTitleModelView>> GetPagedAsync(string search, ActiveFilter active, int page, int pageSize, Guid? companyId = null);
}