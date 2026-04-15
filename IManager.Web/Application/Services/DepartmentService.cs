using AutoMapper;
using IManager.Web.Application.Interfaces;
using IManager.Web.Domain.Entities.Companies;
using IManager.Web.Domain.Interfaces.Persistence;
using IManager.Web.Domain.Interfaces.Repositories;
using IManager.Web.Presentation.ViewModels.Companies;
using IManager.Web.Presentation.ViewModels.Departments;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace IManager.Web.Application.Services;

public class DepartmentService : IDepartmentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IRepository<Department> _departmentRepository;
    public DepartmentService(IUnitOfWork unitOfWork, IMapper mapper, IRepository<Department> departmentRepository)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _departmentRepository = departmentRepository;
    }

    public async Task<IEnumerable<DepartmentHierarchyViewModel>> GetDepartmentsHierarchyViewModelAsync(Guid? companyId = null)
    {
        IEnumerable<Department> departments;
        if (companyId is null)
            departments = await _departmentRepository.GetAllAsync(q => q.Include(d => d.JobTitles));
        else
            departments = await _departmentRepository.GetAllAsync(q =>
                                        q.Where(d => d.CompanyId == companyId)
                                        .Include(d => d.JobTitles));

        var departmentsHierarchy = _mapper.Map<IEnumerable<DepartmentHierarchyViewModel>>(departments);
        return departmentsHierarchy;
    }

    public async Task<PagedResult<DepartmentViewModel>> GetPagedAsync(string search, ActiveFilter active, int page, int pageSize, Guid? companyId = null)
    {
        Func<IQueryable<Department>, IQueryable<Department>> query = q =>
        {
            q = q.Include(d => d.Company);

            if (companyId.HasValue)
            {
                q = q.Where(d => d.CompanyId == companyId);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.ToLower();
                q = q.Where(d =>
                    EF.Functions.Like(d.Name.ToLower(), $"%{search}%") ||
                    EF.Functions.Like(d.Company.TradeName.ToLower(), $"%{search}%") ||
                    EF.Functions.Like(d.Company.DocumentNumber.ToLower(), $"%{search}%")
                );
            }

            switch (active)
            {
                case ActiveFilter.Active:
                    q = q.Where(d => d.IsActive);
                    break;
                case ActiveFilter.Inactive:
                    q = q.Where(d => !d.IsActive);
                    break;
                default:
                    break;
            }

            return q;
        };

        var totalCount = await _departmentRepository.CountAsync(query);

        IEnumerable<Department> departments = await _departmentRepository.GetAllAsync(query, page, pageSize);

        var departmentDetailsViewModel = _mapper.Map<IEnumerable<DepartmentViewModel>>(departments);

        var pagedViewModel = new PagedResult<DepartmentViewModel>()
        {
            Items = departmentDetailsViewModel,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            Search = search
        };

        return pagedViewModel;
    }
}