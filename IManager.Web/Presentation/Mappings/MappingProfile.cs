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
        CreateMap<CompanySeedData, Company>().ReverseMap().MaxDepth(5);
        CreateMap<DepartmentSeedData, Department>().ReverseMap().MaxDepth(5);
        CreateMap<JobTitleSeedData, JobTitle>().ReverseMap().MaxDepth(5);

        CreateMap<User, AccountDetailsViewModel>().ReverseMap().MaxDepth(5);
        CreateMap<UserSeedData, UserProfile>().ReverseMap().MaxDepth(5);
        CreateMap<UserProfile, AccountViewModel>().ReverseMap().MaxDepth(5);
        CreateMap<UserProfile, AccountDetailsViewModel>()
            .ForMember(dest => dest.JobTitle, opt => opt.MapFrom(src => src.JobTitle.Name))
            .ReverseMap();
        CreateMap<UserProfile, EditAccountViewModel>()
            .ForMember(dest => dest.CompanyId, opt => opt.MapFrom(src => src.JobTitle.Department.Company.Id))
            .ForMember(dest => dest.DepartmentId, opt => opt.MapFrom(src => src.JobTitle.Department.Id))
            .ForMember(dest => dest.JobTitleId, opt => opt.MapFrom(src => src.JobTitle.Id));
        CreateMap<UserProfile, RegisterViewModel>().ReverseMap().MaxDepth(5);

        CreateMap<Company, CompanyViewModel>().ReverseMap().MaxDepth(5);
        CreateMap<Company, CreateCompanyViewModel>().ReverseMap().MaxDepth(5);
        CreateMap<Company, EditCompanyViewModel>().ReverseMap().MaxDepth(5);
        CreateMap<Company, CompanyHierarchyViewModel>().ReverseMap().MaxDepth(5);

        CreateMap<Department, DepartmentViewModel>().ReverseMap().MaxDepth(5);
        CreateMap<Department, CreateDepartmentViewModel>().ReverseMap().MaxDepth(5);
        CreateMap<Department, EditDepartmentViewModel>().ReverseMap().MaxDepth(5);
        CreateMap<Department, DepartmentHierarchyViewModel>().ReverseMap().MaxDepth(5);

        CreateMap<JobTitle, JobTitleModelView>().ReverseMap().MaxDepth(5);
        CreateMap<JobTitle, IndexJobTitleModelView>()
            .ForMember(dest => dest.CompanyId, opt => opt.MapFrom(src => src.Department.Company.Id))
            .ForMember(dest => dest.CompanyTradeName, opt => opt.MapFrom(src=>src.Department.Company.TradeName))
            .ForMember(dest => dest.CompanyDocumentNumber, opt => opt.MapFrom(src => src.Department.Company.DocumentNumber))
            .ForMember(dest => dest.DepartmentId, opt => opt.MapFrom(src => src.Department.Id))
            .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Department.Name))
            .ReverseMap().MaxDepth(5);

        CreateMap<JobTitle, JobTitleHierarchyModelView>().ReverseMap().MaxDepth(5);

        CreateMap<TimeEntry, TimeEntryDTO>().ReverseMap().MaxDepth(5);
        CreateMap<TimeCheck, TimeCheckDTO>().ReverseMap().MaxDepth(5);
    }
}