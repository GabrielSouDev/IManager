using FluentValidation;
using IManager.Web.Presentation.ViewModels.Account;

namespace IManager.Web.Presentation.Validators.Account;

public class ResetPasswordViewModelValidator : AbstractValidator<ResetPasswordViewModel>
{
    public ResetPasswordViewModelValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("O e-mail é obrigatório.")
            .EmailAddress().WithMessage("Digite um e-mail válido.");

        RuleFor(x => x.Token)
            .NotEmpty();

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("A senha é obrigatória.")
            .MinimumLength(6).WithMessage("A senha deve ter pelo menos 6 caracteres.");

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty().WithMessage("Confirme a senha.")
            .Equal(x => x.NewPassword).WithMessage("As senhas não coincidem.");
    }
}