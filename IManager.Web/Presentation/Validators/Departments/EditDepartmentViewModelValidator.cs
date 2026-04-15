using FluentValidation;
using IManager.Web.Presentation.Validators.Shared;
using IManager.Web.Presentation.ViewModels.Companies;
using IManager.Web.Presentation.ViewModels.Departments;
using IManager.Web.Presentation.ViewModels.JobTitles;

namespace IManager.Web.Presentation.Validators.Departments;

public class EditDepartmentViewModelValidator : AbstractValidator<EditDepartmentViewModel>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public EditDepartmentViewModelValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("A empresa é obrigatória.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("O Nome do setor é obrigatorio.");
    }
}
