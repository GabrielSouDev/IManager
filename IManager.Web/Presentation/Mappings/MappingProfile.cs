using AutoMapper;
using IManager.Web.Data.Seeder.SeedDatas;
using IManager.Web.Domain.Entities.Companies;
using IManager.Web.Domain.Entities.TimeTrackings;
using IManager.Web.Domain.Entities.Users;
using IManager.Web.Presentation.ViewModels.Account;
using IManager.Web.Presentation.ViewModels.Companies;
using IManager.Web.Presentation.ViewModels.Departments;
using IManager.Web.Presentation.ViewModels.JobTitles;
using IManager.Web.Shared.DTO.TimeTrackings;

namespace IManager.Web.Presentation.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<CompanySeedData, Company>();
        CreateMap<DepartmentSeedData, Department>();
        CreateMap<JobTitleSeedData, JobTitle>();
        CreateMap<UserSeedData, UserProfile>();

        CreateMap<UserProfile, AccountViewModel>().ReverseMap();
        CreateMap<UserProfile, AccountDetailsViewModel>()
            .ForMember(dest => dest.JobTitle, opt => opt.MapFrom(src => src.JobTitle.Name))
            .ReverseMap();
        CreateMap<User, AccountDetailsViewModel>().ReverseMap();
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
        CreateMap<Department, EditDepartmentViewModel>().ReverseMap();
        CreateMap<Department, DepartmentHierarchyViewModel>().ReverseMap();

        CreateMap<JobTitle, JobTitleModelView>().ReverseMap();
        CreateMap<JobTitle, IndexJobTitleModelView>()
            .ForMember(dest => dest.CompanyId, opt => opt.MapFrom(src => src.Department.Company.Id))
            .ForMember(dest => dest.CompanyTradeName, opt => opt.MapFrom(src=>src.Department.Company.TradeName))
            .ForMember(dest => dest.CompanyDocumentNumber, opt => opt.MapFrom(src => src.Department.Company.DocumentNumber))
            .ForMember(dest => dest.DepartmentId, opt => opt.MapFrom(src => src.Department.Id))
            .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Department.Name))
            .ReverseMap();

        CreateMap<JobTitle, JobTitleHierarchyModelView>().ReverseMap();

        CreateMap<TimeEntry, TimeEntryDTO>().ReverseMap();
        CreateMap<TimeCheck, TimeCheckDTO>().ReverseMap();
    }
}