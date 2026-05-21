using IManager.Web.Data.Persistence;
using IManager.Web.Domain.Entities.Users;
using IManager.Web.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
namespace IManager.Web.Data.Repositories;

public class UserProfilesRepository : Repository<UserProfile>, IUserProfilesRepository
{
    public UserProfilesRepository(ApplicationDbContext context) : base(context) { }

    public async Task<InfoUserProfileViewModel?> GetAnnualSalaryByIdAsync(Guid id)
    {
        var exists = await ExistsAsync(id);
        if (!exists) return null;

        var start = new DateTime(DateTime.UtcNow.Year - 1, 1, 1);
        var end = start.AddYears(1);

        var result = await _dbSet
            .Where(u => u.Id == id)
            .Select(u => new InfoUserProfileViewModel()
            {
                LastAnnualSalary = u.Payslips
                    .Where(p => p.CreatedAt >= start && p.CreatedAt < end)
                    .Sum(p => (decimal?)p.NetSalary) ?? 0,

                LastAnnualCost = u.Payslips
                    .Where(p => p.CreatedAt >= start && p.CreatedAt < end)
                    .Sum(p => (decimal?)p.GrossSalary) ?? 0
            }).FirstOrDefaultAsync();

        return result;
    }
}

public class InfoUserProfileViewModel
{
    public decimal LastAnnualSalary { get; set; }
    public decimal LastAnnualCost { get; set; }
    public decimal AverageSalary { get; set; }
}