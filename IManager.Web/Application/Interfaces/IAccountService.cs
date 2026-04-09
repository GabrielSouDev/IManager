using IManager.Web.Domain.Entities.Users;
using IManager.Web.Presentation.ViewModels.Account;
using IManager.Web.Presentation.ViewModels.Companies;
using IManager.Web.Presentation.ViewModels.Departments;
using IManager.Web.Shared;

namespace IManager.Web.Application.Interfaces;

public interface IAccountService
{
    Task<Result> RegisterAsync(RegisterViewModel model);
    Task<Result> LoginAsync(string email, string password);
    Task LogoutAsync();
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByIdAsync(string userId);
    Task<string> GenerateConfirmationTokenAsync(User user);
    Task SendConfirmationEmailAsync(User user, string link);
    Task<Result> ConfirmEmailAsync(Guid userId, string token);
    Task<string> GenerateResetPasswordTokenAsync(User user);
    Task SendResetPasswordEmailAsync(User user, string link);
    Task<Result> ConfirmResetPasswordTokenAsync(ResetPasswordViewModel model);
    Task<string> GenerateChangeEmailTokenAsync(User user, string newEmail);
    Task<Result> ConfirmChangeEmailAsync(string currentEmail, string newEmail, string token);
    Task SendChangeEmailLinkAsync(User user, string newEmail, string link);
    Task<AccountDetailsViewModel?> GetAccountDetailsViewModelByEmailAsync(string email);
    Task<Result> EditDetailsAsync(string email, AccountDetailsViewModel model);
    Task<Result> ChangePasswordAsync(string email, ChangePasswordViewModel model);
    Task<IEnumerable<CompanyHierarchyViewModel>> GetCompaniesHierarchyViewModelAsync();
    Task<IEnumerable<DepartmentHierarchyViewModel>> GetDepartmentsHierarchyViewModelAsync(Guid? companyId = null);
    Task<IEnumerable<AccountViewModel>> GetAllAccountViewModelAsync(Guid? companyId = null, string? search = null);
    Task<EditAccountViewModel> GetEditAccountViewModelByIdAsync(Guid id);
}