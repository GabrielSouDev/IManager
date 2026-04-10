using AutoMapper;
using IManager.Web.Application.Interfaces;
using IManager.Web.Domain.Entities.Companies;
using IManager.Web.Domain.Interfaces.Persistence;
using IManager.Web.Domain.Interfaces.Repositories;
using IManager.Web.Presentation.ViewModels.Companies;
using Microsoft.EntityFrameworkCore;


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

    public async Task<IEnumerable<CompanyHierarchyViewModel>> GetCompaniesHierarchyViewModelAsync()
    {
        var companies = await _companyRepository.GetAllAsync(q => q.Include(c => c.Departments)
                                                                   .ThenInclude(d => d.JobTitles));

        var companiesHierarchy = _mapper.Map<IEnumerable<CompanyHierarchyViewModel>>(companies);
        return companiesHierarchy;
    }
}