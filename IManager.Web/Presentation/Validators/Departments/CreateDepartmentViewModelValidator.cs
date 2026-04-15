using FluentValidation;
using IManager.Web.Presentation.ViewModels.Companies;
using IManager.Web.Presentation.ViewModels.Departments;
using IManager.Web.Presentation.ViewModels.JobTitles;

namespace IManager.Web.Presentation.Validators.Departments;

public class CreateDepartmentViewModelValidator : AbstractValidator<CreateDepartmentViewModel>
{
    public string Name { get; set; } = string.Empty;
    public Guid CompanyId { get; set; }

    public CreateDepartmentViewModelValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("O Nome do setor é obrigatorio.");

        RuleFor(x => x.CompanyId)
        .NotEmpty().WithMessage("A empresa é obrigatória.");
    }
}
