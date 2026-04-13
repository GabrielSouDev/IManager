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

    public async Task<CompanyViewModel> GetByIdAsync(Guid? id)
    {
        var company = await _companyRepository.GetByIdAsync(id!.Value);

        return _mapper.Map<CompanyViewModel>(company);
    }

    public async Task<IEnumerable<CompanyHierarchyViewModel>> GetCompaniesHierarchyViewModelAsync()
    {
        var companies = await _companyRepository.GetAllAsync(q => q.Include(c => c.Departments)
                                                                   .ThenInclude(d => d.JobTitles));

        var companiesHierarchy = _mapper.Map<IEnumerable<CompanyHierarchyViewModel>>(companies);
        return companiesHierarchy;
    }

    public async Task<PagedResult<CompanyViewModel>> GetPagedAsync(int page, int pageSize, string search)
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
}