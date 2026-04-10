using AutoMapper;
using IManager.Web.Application.Interfaces;
using IManager.Web.Domain.Entities.Companies;
using IManager.Web.Domain.Interfaces.Persistence;
using IManager.Web.Domain.Interfaces.Repositories;
using IManager.Web.Presentation.ViewModels.Departments;
using Microsoft.EntityFrameworkCore;

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
}