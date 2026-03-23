using FluentValidation;
using IManager.Web.Presentation.ViewModels.Account;

namespace IManager.Web.Presentation.Validators.Account;

public class ChangeEmailViewModelValidator : AbstractValidator<ChangeEmailViewModel>
{
    public ChangeEmailViewModelValidator()
    {
        RuleFor(x => x.NewEmail)
            .NotEmpty().WithMessage("O e-mail é obrigatório")
            .EmailAddress().WithMessage("Digite um e-mail válido");
    }
}
