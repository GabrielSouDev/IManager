using FluentValidation;
using IManager.Web.Presentation.ViewModels.Account;

namespace IManager.Web.Presentation.Validators.Account;

public class ChangePasswordViewModelValidator : AbstractValidator<ChangePasswordViewModel>
{
    public ChangePasswordViewModelValidator()
    {
        RuleFor(x => x.OldPassword)
            .NotEmpty().WithMessage("A senha antiga é obrigatória");

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("A nova senha é obrigatória");

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty().WithMessage("Confirme a nova senha")
            .Equal(x => x.NewPassword).WithMessage("A senhas não coincidem");
    }
}
