using FluentValidation;
using IManager.Web.Presentation.Validators.Shared;
using IManager.Web.Presentation.ViewModels.Account;

namespace IManager.Web.Presentation.Validators.Account;

public class AccountDetailsViewModelValidator : AbstractValidator<AccountDetailsViewModel>
{
    public AccountDetailsViewModelValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("O Nome Completo é obrigatório.")
            .Matches(@"^[A-Za-zÀ-ÿ]+\s[A-Za-zÀ-ÿ\s]+$").WithMessage("Digite o nome e sobrenome.");

        RuleFor(x => x.DocumentNumber)
            .NotEmpty().WithMessage("O CPF é obrigatório.")
            .Must(DocumentValidator.IsCpfValid).WithMessage("CPF Inválido.");

        RuleFor(x => x.BirthDate)
            .NotNull().WithMessage("A Data de Nascimento é obrigatória.");

        RuleFor(x => x.PhoneNumber)
            .Matches(@"^\(\d{2}\) \d{4,5}-\d{4}$").WithMessage("Digite um telefone válido.")
            .When(x => !string.IsNullOrEmpty(x.PhoneNumber));
    }
}