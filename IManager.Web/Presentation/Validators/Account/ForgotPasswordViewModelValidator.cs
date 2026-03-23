using FluentValidation;
using IManager.Web.Presentation.ViewModels.Account;

namespace IManager.Web.Presentation.Validators.Account;

public class ForgotPasswordViewModelValidator : AbstractValidator<ForgotPasswordViewModel>
{
    public ForgotPasswordViewModelValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Digite o e-mail do cadastro.")
            .EmailAddress().WithMessage("E-mail Invalido.");
    }
}