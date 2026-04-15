using AutoMapper;
using IManager.Web.Application.Interfaces;
using IManager.Web.Data.Repositories;
using IManager.Web.Domain.Entities.Companies;
using IManager.Web.Domain.Entities.Users;
using IManager.Web.Domain.Interfaces.Persistence;
using IManager.Web.Domain.Interfaces.Repositories;
using IManager.Web.Presentation.ViewModels.Account;
using IManager.Web.Presentation.ViewModels.Companies;
using IManager.Web.Shared;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.Design;


namespace IManager.Web.Application.Services;

public class CompanyService : ICompanyService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IRepository<Company> _companyRepository;

    public CompanyService(IUnitOfWork unitOfWork, IMapper mapper, IRepository<Company> companyRepository)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _companyRepository = companyRepository;
    }

    public async Task<Result> AddAsync(CreateCompanyViewModel company)
    {
        var exists = await _companyRepository.ExistsAsync(c=>c.DocumentNumber == company.DocumentNumber);

        if (exists)
            return Result.Fail("Empresa já esta cadastrada.");

        try
        {
            var entity = _mapper.Map<Company>(company);
            await _companyRepository.AddAsync(entity);
        }
        catch (Exception ex)
        {

            return Result.Fail(ex.Message);
        }

        return Result.Ok();
    }

    public async Task<Result> SoftDeleteAsync(Guid id)
    {
        if (id == Guid.Empty) return Result.Fail("Empresa não localizada.");

        var exists = await _companyRepository.ExistsAsync(c => c.Id == id);

        if(!exists) return Result.Fail("Empresa não localizada.");

        try
        {
            await _companyRepository.SoftDeleteAsync(id);
            return Result.Ok();
        }
        catch (Exception)
        {
            return Result.Fail("Falha ao desativar empresa.");
        }
    }

    public async Task<IEnumerable<CompanyHierarchyViewModel>> GetCompaniesHierarchyViewModelAsync()
    {
        var companies = await _companyRepository.GetAllAsync(q => q.Include(c => c.Departments)
                                                                   .ThenInclude(d => d.JobTitles));

        var companiesHierarchy = _mapper.Map<IEnumerable<CompanyHierarchyViewModel>>(companies);
        return companiesHierarchy;
    }

    public async Task<EditCompanyViewModel?> GetEditViewModelByIdAsync(Guid id)
    {
        var entity = await _companyRepository.GetByIdAsync(id);
        if (entity == null) return null;

        var model = _mapper.Map<EditCompanyViewModel>(entity);
        return model;
    }

    public async Task<PagedResult<CompanyViewModel>> GetPagedAsync(string search, ActiveFilter active, int page, int pageSize)
    {
        Func<IQueryable<Company>, IQueryable<Company>> query = q =>
        {
            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.ToLower();
                q = q.Where(u =>
                    EF.Functions.Like(u.LegalName.ToLower(), $"%{search}%") ||
                    EF.Functions.Like(u.DocumentNumber.ToLower(), $"%{search}%") ||
                    EF.Functions.Like(u.TradeName.ToLower(), $"%{search}%")
                );
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

            return q;
        };

        var totalCount = await _companyRepository.CountAsync(query);

        IEnumerable<Company> userProfiles = await _companyRepository.GetAllAsync(query, page, pageSize);
        var CompanyDetailsViewModel = _mapper.Map<IEnumerable<CompanyViewModel>>(userProfiles);

        var pagedViewModel = new PagedResult<CompanyViewModel>()
        {
            Items = CompanyDetailsViewModel,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            Search = search
        };

        return pagedViewModel;
    }

    public async Task<CompanyViewModel?> GetViewModelByIdAsync(Guid id)
    {
        var entity = await _companyRepository.GetByIdAsync(id);
        if (entity == null) return null;

        var model = _mapper.Map<CompanyViewModel>(entity);
        return model;
    }

    public async Task<Result> UpdateAsync(Guid id,EditCompanyViewModel company)
    {
        try
        {
            var entity = await _companyRepository.GetByIdAsync(id);
            if (entity is null && company.Id != id) Result.Fail("Empresa não encontrada.");

            _mapper.Map(company, entity);
            await _companyRepository.UpdateAsync(entity!);
            return Result.Ok();
        }
        catch (Exception)
        {
            return Result.Fail("Erro ao atualizar empresa.");
        }
    }

    public async Task<IEnumerable<CompanyViewModel>?> GetCompaniesViewModelsAsync()
    {
        var departments = await _companyRepository.GetAllAsync();

        return _mapper.Map<IEnumerable<CompanyViewModel>>(departments) ?? new List<CompanyViewModel>();
    }
}