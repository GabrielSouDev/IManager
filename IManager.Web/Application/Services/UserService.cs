using AutoMapper;
using IManager.Web.Application.Interfaces;
using IManager.Web.Domain.Entities.Users;
using IManager.Web.Domain.Interfaces.Repositories;
using IManager.Web.Presentation.ViewModels.Account;
using IManager.Web.Presentation.ViewModels.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IManager.Web.Application.Services;

public class UserService : IUserService
{
    private readonly IUserProfilesRepository _userProfileRepository;
    private readonly UserManager<User> _userManager;
    private readonly IMapper _mapper;

    public UserService(IUserProfilesRepository userProfileRepository, UserManager<User> userManager, IMapper mapper)
    {
        _userProfileRepository = userProfileRepository;
        _userManager = userManager;
        _mapper = mapper;
    }

    public async Task<PagedResult<IndexUserViewModel>> GetPagedAsync(
        int page, int pageSize, ActiveFilter active, Guid? companyId = null, string? search = null)
    {
        Func<IQueryable<UserProfile>, IQueryable<UserProfile>> query = q =>
        {
            q = q
                .Include(u => u.JobTitle)
                .ThenInclude(j => j.Department)
                .ThenInclude(d => d.Company);

            if (companyId.HasValue)
            {
                q = q.Where(u => u.CompanyId == companyId);
            }

            switch (active)
            {
                case ActiveFilter.Active:
                    q = q.Where(u => u.IsActive);
                    break;
                case ActiveFilter.Inactive:
                    q = q.Where(u => !u.IsActive);
                    break;
                default:
                    break;
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.ToLower();
                q = q.Where(u =>
                    EF.Functions.Like(u.FullName.ToLower(), $"%{search}%") ||
                    EF.Functions.Like(u.DocumentNumber.ToLower(), $"%{search}%") ||
                    EF.Functions.Like(u.Company.TradeName.ToLower(), $"%{search}%") ||
                    EF.Functions.Like(u.JobTitle.Name.ToLower(), $"%{search}%") ||
                    EF.Functions.Like(u.Role.ToLower(), $"%{search}%")
                );
            }
            return q;
        };

        var totalCount = await _userProfileRepository.CountAsync(query);

        IEnumerable<UserProfile> userProfiles = await _userProfileRepository.GetAllAsync(query, page, pageSize);
        var accountsDetailsViewModel = _mapper.Map<IEnumerable<IndexUserViewModel>>(userProfiles);

        var pagedViewModel = new PagedResult<IndexUserViewModel>()
        {
            Items = accountsDetailsViewModel,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            Search = search,
            ActiveFilter = active
        };

        return pagedViewModel;
    }

    public async Task<DetailsUserViewModel?> GetDetailsViewModelByIdAsync(Guid id)
    {
        var user = await GetByIdAsync(id.ToString());
        if (user == null) return null;

        var userProfile = await _userProfileRepository.GetByIdAsync(user.Id, q => q.Include(u => u.JobTitle));
        if (userProfile == null) return null;

        var model = _mapper.Map<DetailsUserViewModel>(userProfile);
        _mapper.Map(user, model);

        var info = await _userProfileRepository.GetInfoByIdAsync(id);
        if (info != null)
            model.Info = info;

        return model;

    }

    public async Task<User?> GetByIdAsync(string userId)
=> await _userManager.FindByIdAsync(userId);
}
