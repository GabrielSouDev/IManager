using AutoMapper;
using IManager.Web.Application.Interfaces;
using IManager.Web.Domain.Entities.Companies;
using IManager.Web.Domain.Interfaces.Persistence;
using IManager.Web.Domain.Interfaces.Repositories;
using IManager.Web.Presentation.ViewModels.JobTitles;
using IManager.Web.Shared;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace IManager.Web.Application.Services;

public class JobTitleService : IJobTitleService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IRepository<JobTitle> _jobTitleRepository;

    public JobTitleService(IUnitOfWork unitOfWork, IMapper mapper, IRepository<JobTitle> jobTitleRepository)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _jobTitleRepository = jobTitleRepository;
    }

    public async Task<Result> AddJobTitle(CreateJobTitleModelView model)
    {
        var entity = _mapper.Map<JobTitle>(model);

        try
        {
            await _jobTitleRepository.AddAsync(entity);
        }
        catch (Exception)
        {
            return Result.Fail("Por favor, tente novamente.");
        }

        return Result.Ok();
    }

    public async Task<DetailsJobTitleModelView> GetDetailsModelView(Guid id)
    {
        var entity = await _jobTitleRepository.GetByIdAsync(id, q => q.Include(j => j.Department).ThenInclude(d=>d.Company));

        var model = _mapper.Map<DetailsJobTitleModelView?>(entity);

        return model ?? new();
    }

    public async Task<PagedResult<IndexJobTitleModelView>> GetPagedAsync(string search, ActiveFilter active, int page, int pageSize, Guid? companyId = null)
    {
        Func<IQueryable<JobTitle>, IQueryable<JobTitle>> query = q =>
        {
            q = q.Include(j => j.Department)
                 .ThenInclude(d=> d.Company);

            if (companyId.HasValue)
            {
                q = q.Where(j => j.Department.Company.Id == companyId);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.ToLower();
                q = q.Where(d =>
                    EF.Functions.Like(d.Name.ToLower(), $"%{search}%") ||
                    EF.Functions.Like(d.Department.Name.ToLower(), $"%{search}%") ||
                    EF.Functions.Like(d.Department.Company.DocumentNumber.ToLower(), $"%{search}%") ||
                    EF.Functions.Like(d.Department.Company.TradeName.ToLower(), $"%{search}%") ||
                    EF.Functions.Like(d.Department.Company.LegalName.ToLower(), $"%{search}%")
                );
            }

            switch (active)
            {
                case ActiveFilter.Active:
                    q = q.Where(j => j.IsActive);
                    break;
                case ActiveFilter.Inactive:
                    q = q.Where(j => !j.IsActive);
                    break;
                default:
                    break;
            }

            return q;
        };

        var totalCount = await _jobTitleRepository.CountAsync(query);

        IEnumerable<JobTitle> jobTitles = await _jobTitleRepository.GetAllAsync(query, page, pageSize);

        var indexModelView = _mapper.Map<IEnumerable<IndexJobTitleModelView>>(jobTitles);

        var pagedViewModel = new PagedResult<IndexJobTitleModelView>()
        {
            Items = indexModelView,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            Search = search,
            ActiveFilter = active
        };

        return pagedViewModel;
    }
}