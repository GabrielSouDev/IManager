using AutoMapper;
using IManager.Web.Application.Interfaces;
using IManager.Web.Domain.Entities.Companies;
using IManager.Web.Domain.Interfaces.Persistence;
using IManager.Web.Domain.Interfaces.Repositories;
using IManager.Web.Presentation.ViewModels.Companies;
using IManager.Web.Presentation.ViewModels.Departments;
using IManager.Web.Shared;
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

    public async Task<DepartmentViewModel?> GetViewModelByIdAsync(Guid id)
    {
        var department = await _departmentRepository.GetByIdAsync(id, q => q.Include(d => d.Company));
        var model = _mapper.Map<DepartmentViewModel?>(department);
        return model;
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

    public async Task<Result> AddAsync(CreateDepartmentViewModel department)
    {
        if(string.IsNullOrEmpty(department.Name))
            return Result.Fail("O nome do setor vazio.");

        var exists = await _departmentRepository.ExistsAsync(d => d.CompanyId == department.CompanyId && d.Name == department.Name);

        if (exists)
            return Result.Fail("Setor já existe.");

        var entity = _mapper.Map<Department>(department);
        try
        {
            await _departmentRepository.AddAsync(entity);
        }
        catch (Exception)
        {
            return Result.Fail("Falha ao criar Setor, por favor tente novamente.");
        }

        return Result.Ok();
    }

    public async Task<EditDepartmentViewModel> GetEditViewModelByIdAsync(Guid value)
    {
        var entity = await _departmentRepository.GetByIdAsync(value);

        return _mapper.Map<EditDepartmentViewModel>(entity);
    }

    public async Task<Result> UpdateAsync(EditDepartmentViewModel department)
    {
        var entity = await _departmentRepository.GetByIdAsync(department.Id);
        if(entity is null)
            return Result.Fail("Setor não encontrado.");

        _mapper.Map(department, entity);

        try
        {
            await _departmentRepository.UpdateAsync(entity);
        }
        catch (Exception)
        {
            return Result.Fail("Falha ao atualizar Setor, por favor tente novamente.");
        }

        return Result.Ok();
    }

    public async Task<Result> SoftDeleteAsync(Guid id)
    {
        var exists = await _departmentRepository.ExistsAsync(d => d.Id == id);

        if (!exists)
            return Result.Fail("Setor não encontrado.");

        try
        {
            await _departmentRepository.SoftDeleteAsync(id);
        }
        catch (Exception)
        {
            return Result.Fail("Falha ao excluir Setor, por favor tente novamente.");
        }

        return Result.Ok();
    }
}