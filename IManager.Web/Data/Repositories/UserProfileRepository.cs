using IManager.Web.Data.Persistence;
using IManager.Web.Domain.Entities.Users;
using IManager.Web.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
namespace IManager.Web.Data.Repositories;

public class UserProfileRepository : IUserProfileRepository
{
    private readonly ApplicationDbContext _context;
    public UserProfileRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task AddAsync(UserProfile entity)
    {
        await _context.UserProfiles.AddAsync(entity);
        await _context.SaveChangesAsync();
    }
    public async Task AddRangeAsync(IEnumerable<UserProfile> entities)
    {
        await _context.UserProfiles.AddRangeAsync(entities);
        await _context.SaveChangesAsync();
    }
    public async Task<int> CountAsync(Func<IQueryable<UserProfile>, IQueryable<UserProfile>>? queryBuilder = null)
    {
        IQueryable<UserProfile> query = _context.UserProfiles;

        if (queryBuilder != null)
        {
            query = queryBuilder(query);
        }

        return await query.CountAsync();
    }
    public async Task DeleteAsync(Guid id)
    {
        var entity = await GetByIdAsync(id);
        if (entity is not null)
        {
            entity.Deactivate();
            _context.UserProfiles.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
    public async Task DeleteAsync(UserProfile entity)
    {
        entity.Deactivate();
        _context.UserProfiles.Update(entity);
        await _context.SaveChangesAsync();
    }
    public async Task DeleteRangeAsync(IEnumerable<UserProfile> entities)
    {
        foreach (var entity in entities)
            entity.Deactivate();
        await _context.SaveChangesAsync();
    }
    public async Task<bool> ExistsAsync(Expression<Func<UserProfile, bool>> predicate)
        => await _context.UserProfiles.AnyAsync(predicate);
    public async Task<IEnumerable<UserProfile>> FindAsync(
        Expression<Func<UserProfile, bool>> predicate,
        Func<IQueryable<UserProfile>, IQueryable<UserProfile>>? include = null)
    {
        IQueryable<UserProfile> query = _context.UserProfiles;
        if (include != null)
            query = include(query);
        return await query.Where(predicate).ToListAsync();
    }
    public async Task<UserProfile?> FirstOrDefaultAsync(
        Expression<Func<UserProfile, bool>> predicate,
        Func<IQueryable<UserProfile>, IQueryable<UserProfile>>? include = null)
    {
        IQueryable<UserProfile> query = _context.UserProfiles;
        if (include != null)
            query = include(query);
        return await query.FirstOrDefaultAsync(predicate);
    }
    public async Task<IEnumerable<UserProfile>> GetAllAsync(
        Func<IQueryable<UserProfile>, IQueryable<UserProfile>>? include = null,
        int? page = null,
        int? pageSize = null)
    {
        IQueryable<UserProfile> query = _context.UserProfiles;
        if (include != null)
            query = include(query);

        if (page.HasValue && pageSize.HasValue)
            query = query
                .OrderBy(e => EF.Property<object>(e, "Id"))
                .Skip((page.Value - 1) * pageSize.Value)
                         .Take(pageSize.Value);

        return await query.ToListAsync();
    }
    public async Task<UserProfile?> GetByIdAsync(Guid id, Func<IQueryable<UserProfile>, IQueryable<UserProfile>>? include = null)
    {
        if (include == null)
            return await _context.UserProfiles.FindAsync(id);
        return await include(_context.UserProfiles).FirstOrDefaultAsync(e => e.Id == id);
    }
    public async Task UpdateAsync(UserProfile entity)
    {
        entity.Touch();
        _context.UserProfiles.Update(entity);
        await _context.SaveChangesAsync();
    }
}