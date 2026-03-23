using AutoMapper;
using IManager.Web.Domain.Entities.Companies;
using IManager.Web.Domain.Entities.Users;
using IManager.Web.Presentation.ViewModels.Account;
using IManager.Web.Presentation.ViewModels.Companies;
using IManager.Web.Presentation.ViewModels.Departments;
using IManager.Web.Presentation.ViewModels.JobTitles;

namespace IManager.Web.Presentation.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<UserProfile, AccountViewModel>().ReverseMap();
        CreateMap<UserProfile, EditAccountViewModel>()
            .ForMember(dest => dest.CompanyId, opt => opt.MapFrom(src => src.JobTitle.Department.Company.Id))
            .ForMember(dest => dest.DepartmentId, opt => opt.MapFrom(src => src.JobTitle.Department.Id))
            .ForMember(dest => dest.JobTitleId, opt => opt.MapFrom(src => src.JobTitle.Id));

        CreateMap<Company, CompanyViewModel>().ReverseMap();
        CreateMap<Company, CreateCompanyViewModel>().ReverseMap();
        CreateMap<Company, EditCompanyViewModel>().ReverseMap();
        CreateMap<Company, CompanyHierarchyViewModel>().ReverseMap();

        CreateMap<Department, DepartmentViewModel>().ReverseMap();
        CreateMap<Department, CreateDepartmentViewModel>().ReverseMap();
        CreateMap<Department, DepartmentHierarchyViewModel>().ReverseMap();

        CreateMap<JobTitle, JobTitleModelView>().ReverseMap();
        CreateMap<JobTitle, JobTitleHierarchyModelView>().ReverseMap();
    }
}
