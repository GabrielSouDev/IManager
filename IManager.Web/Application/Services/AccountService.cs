using AutoMapper;
using IManager.Web.Application.Interfaces;
using IManager.Web.Domain.Entities.Companies;
using IManager.Web.Domain.Entities.Users;
using IManager.Web.Domain.Interfaces.Persistence;
using IManager.Web.Domain.Interfaces.Repositories;
using IManager.Web.Presentation.ViewModels.Account;
using IManager.Web.Presentation.ViewModels.Companies;
using IManager.Web.Presentation.ViewModels.Departments;
using IManager.Web.Shared;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using UserProfile = IManager.Web.Domain.Entities.Users.UserProfile;


namespace IManager.Web.Application.Services;

public class AccountService : IAccountService
{
    private readonly SignInManager<User> _signInManager;
    private readonly UserManager<User> _userManager;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmailSender<User> _emailSender;
    private readonly IMapper _mapper;
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly IRepository<Company> _companyRepository;
    private readonly IRepository<Department> _departmentRepository;
    private readonly IRepository<JobTitle> _jobTitleRepository;

    public AccountService(SignInManager<User> signInManager, UserManager<User> userManager, 
        IUnitOfWork unitOfWork, IEmailSender<User> emailSender, IMapper mapper, 
        IUserProfileRepository userProfileRepository, IRepository<Company> companyRepository, 
        IRepository<Department> departmentRepository, IRepository<JobTitle> jobTitleRepository)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _unitOfWork = unitOfWork;
        _emailSender = emailSender;
        _mapper = mapper;
        _userProfileRepository = userProfileRepository;
        _companyRepository = companyRepository;
        _departmentRepository = departmentRepository;
        _jobTitleRepository = jobTitleRepository;
    }





    #region Registro e Confirmação de E-mail

    public async Task<Result> RegisterAsync(RegisterViewModel model)
    {
        await _unitOfWork.BeginTransactionAsync();
        try
        {
            var user = new User { UserName = model.Email, Email = model.Email, PhoneNumber = model.PhoneNumber };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                await _unitOfWork.RollbackAsync();
                return Result.Fail(result.Errors.Select(r => r.Description));
            }

            var userProfile = new UserProfile()
            {
                Id = user.Id,
                CompanyId = model.CompanyId,
                JobTitleId = model.JobTitleId,
                FullName = model.FullName,
                DocumentNumber = model.DocumentNumber,
                BirthDate = model.BirthDate,
                Role = model.Role,
                BaseSalary = model.BaseSalary
            };
            await _userProfileRepository.AddAsync(userProfile);

            var jobTitle = await _jobTitleRepository.GetByIdAsync(userProfile.JobTitleId, q => 
                                q.Include(c => c.Department).ThenInclude(d => d.Company))
                                ?? throw new ArgumentNullException("JobTitle não localizado.");

            await _userManager.AddClaimsAsync(user, new List<Claim>
            {
                new("FullName", userProfile?.FullName ?? "Desconhecido"),
                new("CompanyId", jobTitle.Department.Company.Id.ToString() ?? "Null"),
                new("CompanyTradeName", jobTitle.Department.Company.TradeName.ToString() ?? "Null"),
                new("DepartmentId", jobTitle.Department.Id.ToString() ?? "Null"),
                new("Department", jobTitle.Department.Name.ToString() ?? "Null"),
                new("JobTitleId", jobTitle.Id.ToString() ?? "Null"),
                new("JobTitle", jobTitle.Name.ToString() ?? "Null")
            });

            await _userManager.AddToRoleAsync(user, model.Role);

            await _unitOfWork.CommitAsync();
            return Result.Ok();
        }
        catch
        {
            await _unitOfWork.RollbackAsync();
            return Result.Fail("Erro ao criar conta. Tente novamente.");
        }
    }

    public async Task<string> GenerateConfirmationTokenAsync(User user)
        => await _userManager.GenerateEmailConfirmationTokenAsync(user);

    public async Task SendConfirmationEmailAsync(User user, string link)
        => await _emailSender.SendConfirmationLinkAsync(user, user.Email!, link);

    public async Task<Result> ConfirmEmailAsync(Guid userId, string token)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
            return Result.Fail("Usuário não encontrado.");

        var result = await _userManager.ConfirmEmailAsync(user, token);

        if (!result.Succeeded)
            return Result.Fail(result.Errors.Select(r => r.Description));

        return Result.Ok();
    }

    #endregion

    #region Login e Logout

    public async Task<Result> LoginAsync(string email, string password)
    {
        var result = await _signInManager.PasswordSignInAsync(email, password, true, true);

        if (!result.Succeeded)
        {
            if (result.IsLockedOut)
                return Result.Fail("Conta bloqueada. Tente novamente mais tarde.");
            if (result.IsNotAllowed)
                return Result.Fail("Confirme seu e-mail antes de fazer login.");
            return Result.Fail("E-mail ou senha inválidos.");
        }

        return Result.Ok();
    }

    public async Task LogoutAsync()
        => await _signInManager.SignOutAsync();

    #endregion

    #region Recuperação de Senha

    public async Task<string> GenerateResetPasswordTokenAsync(User user)
        => await _userManager.GeneratePasswordResetTokenAsync(user);

    public async Task SendResetPasswordEmailAsync(User user, string link)
        => await _emailSender.SendPasswordResetLinkAsync(user, user.Email!, link);

    public async Task<Result> ConfirmResetPasswordTokenAsync(ResetPasswordViewModel model)
    {
        var user = await GetByEmailAsync(model.Email);
        if (user == null)
            return Result.Fail("Usuário não encontrado.");

        var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);

        if (!result.Succeeded)
            return Result.Fail(result.Errors.Select(r => r.Description));

        return Result.Ok();
    }

    #endregion

    #region Perfil
    public async Task<AccountDetailsViewModel?> GetAccountDetailsViewModelByEmailAsync(string email)
    {
        var user = await GetByEmailAsync(email) ?? throw new ArgumentException("Usuário não encontrado!");

        var userProfile = await _userProfileRepository.GetByIdAsync(user.Id)
            ?? throw new ArgumentException("Usuário não encontrado!");

        return new AccountDetailsViewModel()
        {
            FullName = userProfile.FullName,
            DocumentNumber = userProfile.DocumentNumber,
            BirthDate = userProfile.BirthDate,
            PhoneNumber = user.PhoneNumber
        };
    }

    public async Task<Result> EditDetailsAsync(string email, AccountDetailsViewModel model)
    {
        await _unitOfWork.BeginTransactionAsync();
        try
        {
            var user = await GetByEmailAsync(email);
            if (user == null)
                return Result.Fail("Usuário não encontrado.");

            user.PhoneNumber = model.PhoneNumber;
            await _userManager.UpdateAsync(user);

            var userProfile = await _userProfileRepository.GetByIdAsync(user.Id);
            userProfile!.FullName = model.FullName;
            userProfile.BirthDate = model.BirthDate;
            await _userProfileRepository.UpdateAsync(userProfile);

            var oldClaim = (await _userManager.GetClaimsAsync(user))
                .FirstOrDefault(c => c.Type == "FullName");

            if (oldClaim != null)
                await _userManager.ReplaceClaimAsync(user, oldClaim, new Claim("FullName", model.FullName));
            else
                await _userManager.AddClaimAsync(user, new Claim("FullName", model.FullName));

            await _unitOfWork.CommitAsync();
            return Result.Ok();
        }
        catch
        {
            await _unitOfWork.RollbackAsync();
            return Result.Fail("Erro ao atualizar perfil. Tente novamente.");
        }
    }

    #endregion

    #region Troca de E-mail

    public Task<string> GenerateChangeEmailTokenAsync(User user, string newEmail)
        => _userManager.GenerateChangeEmailTokenAsync(user, newEmail);

    public async Task SendChangeEmailLinkAsync(User user, string newEmail, string link)
        => await _emailSender.SendConfirmationLinkAsync(user, newEmail, link);

    public async Task<Result> ConfirmChangeEmailAsync(string currentEmail, string newEmail, string token)
    {
        var user = await _userManager.Users
            .FirstOrDefaultAsync(u => u.NormalizedEmail == currentEmail.ToUpperInvariant());
        if (user == null)
            return Result.Fail("Usuário não encontrado.");

        var result = await _userManager.ChangeEmailAsync(user, newEmail, token);
        if (!result.Succeeded)
            return Result.Fail(result.Errors.Select(e => e.Description));

        await _userManager.SetUserNameAsync(user, newEmail);
        return Result.Ok();
    }
    #endregion

    #region Troca de Senha

    public async Task<Result> ChangePasswordAsync(string email, ChangePasswordViewModel model)
    {
        var user = await GetByEmailAsync(email);
        if (user == null)
            return Result.Fail("Usuário não encontrado.");

        var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);

        if (!result.Succeeded)
            return Result.Fail(result.Errors.Select(r => r.Description));

        return Result.Ok();
    }
    #endregion

    #region Utils
    public async Task<User?> GetByEmailAsync(string email)
    => await _userManager.Users
    .FirstOrDefaultAsync(u => u.NormalizedEmail == email.ToUpperInvariant());

    public async Task<User?> GetByIdAsync(string userId)
    => await _userManager.FindByIdAsync(userId);

    public async Task<IEnumerable<CompanyHierarchyViewModel>> GetCompaniesHierarchyViewModelAsync()
    {
        var companies = await _companyRepository.GetAllAsync(q => q.Include(c => c.Departments)
                                                                   .ThenInclude(d => d.JobTitles));


        var companiesHierarchy = _mapper.Map<IEnumerable<CompanyHierarchyViewModel>>(companies);
        return companiesHierarchy;
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

    public async Task<IEnumerable<AccountViewModel>> GetAllAccountViewModelAsync(Guid? companyId = null, string? search = null)
    {

        Func<IQueryable<UserProfile>, IQueryable<UserProfile>> query = q =>
        {
            q = q
                .Include(u => u.JobTitle)
                .ThenInclude(j => j.Department)
                .ThenInclude(d => d.Company);

            if (companyId.HasValue)
            {
                q = q.Where(u => u.CompanyId == companyId);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.ToLower();
                q = q.Where(u =>
                    EF.Functions.Like(u.FullName.ToLower(), $"%{search}%") ||
                    EF.Functions.Like(u.DocumentNumber.ToLower(), $"%{search}%") ||
                    EF.Functions.Like(u.Company.TradeName.ToLower(), $"%{search}%") ||
                    EF.Functions.Like(u.JobTitle.Name.ToLower(), $"%{search}%") ||
                    EF.Functions.Like(u.Role.ToLower(), $"%{search}%")
                );
            }
            return q;
        };

        IEnumerable<UserProfile> userProfiles = await _userProfileRepository.GetAllAsync(query);
        var accountsDetailsViewModel = _mapper.Map<IEnumerable<AccountViewModel>>(userProfiles);

        return accountsDetailsViewModel;
    }

    public async Task<EditAccountViewModel> GetEditAccountViewModelByIdAsync(Guid id)
    {
        var userProfile = await _userProfileRepository.GetByIdAsync(id, q => q
                                          .Include(u => u.JobTitle)
                                          .ThenInclude(j => j.Department)
                                          .ThenInclude(d => d.Company));

        var editAccountCiewModel = _mapper.Map<EditAccountViewModel>(userProfile);

        var user = await _userManager.FindByIdAsync(id.ToString());

        editAccountCiewModel.Email = user!.Email!;
        editAccountCiewModel.PhoneNumber = user!.PhoneNumber!;

        return editAccountCiewModel;
    }
    #endregion
}