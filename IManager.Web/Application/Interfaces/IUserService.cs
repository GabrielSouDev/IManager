using IManager.Web.Presentation.ViewModels.Account;
using IManager.Web.Presentation.ViewModels.Users;

namespace IManager.Web.Application.Interfaces;

public interface IUserService
{
    Task<DetailsUserViewModel?> GetDetailsViewModelByIdAsync(Guid id);
    Task<PagedResult<IndexUserViewModel>> GetPagedAsync(int page, int pageSize, ActiveFilter active, Guid? companyId = null, string? search = null);
}