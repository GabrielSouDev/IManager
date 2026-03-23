using FluentValidation;
using IManager.Web.Presentation.Validators.Shared;
using IManager.Web.Presentation.ViewModels.Companies;

namespace IManager.Web.Presentation.Validators.Companies;

public class EditCompanyViewModelValidator : AbstractValidator<EditCompanyViewModel>
{
    public EditCompanyViewModelValidator()
    {
        RuleFor(x => x.DocumentNumber)
            .NotEmpty().WithMessage("O CNPJ é obrigatório.")
            .Must(DocumentValidator.IsCnpjValid).WithMessage("CNPJ Inválido.");

        RuleFor(x => x.LegalName)
            .NotEmpty().WithMessage("A Razão Social é obrigatória");

        RuleFor(x => x.TradeName)
            .NotEmpty().WithMessage("O Nome Fantasia é obrigatória");

        RuleFor(x => x.FoundedAt)
            .NotNull().WithMessage("A Data de Fundação é obrigatória");
    }
}
