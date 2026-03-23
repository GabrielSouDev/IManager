using FluentValidation;
using IManager.Web.Presentation.Validators.Shared;
using IManager.Web.Presentation.ViewModels.Account;

namespace IManager.Web.Presentation.Validators.Account;

public class RegisterViewModelValidator : AbstractValidator<RegisterViewModel>
{
    public RegisterViewModelValidator()
    {
        RuleFor(x => x.CompanyId)
            .NotEmpty().WithMessage("Selecione a empresa.");

        RuleFor(x => x.DepartmentId)
            .NotEmpty().WithMessage("Selecione o Setor.");

        RuleFor(x => x.JobTitleId)
            .NotEmpty().WithMessage("Selecione o Cargo.");

        RuleFor(x => x.BaseSalary)
            .GreaterThanOrEqualTo(0).WithMessage("O Salario Base deve ser positivo.");

        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("O Nome Completo é obrigatório.")
            .Matches(@"^[A-Za-zÀ-ÿ]+\s[A-Za-zÀ-ÿ\s]+$").WithMessage("Digite o nome e sobrenome.");

        RuleFor(x => x.DocumentNumber)
            .NotEmpty().WithMessage("O CPF é obrigatório.")
            .Must(DocumentValidator.IsCpfValid).WithMessage("CPF Inválido.");

        RuleFor(x => x.BirthDate)
            .NotNull().WithMessage("A Data de Nascimento é obrigatória.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("O e-mail é obrigatório.")
            .EmailAddress().WithMessage("Digite um e-mail válido.");

        RuleFor(x => x.PhoneNumber)
            .Matches(@"^\(\d{2}\) \d{4,5}-\d{4}$").WithMessage("Digite um telefone válido.")
            .When(x => !string.IsNullOrEmpty(x.PhoneNumber));

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("A senha é obrigatória.");

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty().WithMessage("Confirme a senha.")
            .Equal(x => x.Password).WithMessage("As senhas não coincidem.");

        RuleFor(x => x.Role)
            .NotEmpty().WithMessage("Perfil de Acesso é obrigatório.");
    }
}