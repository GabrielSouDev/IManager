using IManager.Web.Data.Repositories;
using IManager.Web.Domain.Entities.Users;

namespace IManager.Web.Domain.Interfaces.Repositories;

public interface IUserProfilesRepository : IRepository<UserProfile>
{
    Task<InfoUserProfileViewModel?> GetAnnualSalaryByIdAsync(Guid id);
}