using FluentValidation;
using IManager.Web.Presentation.ViewModels.JobTitles;

namespace IManager.Web.Presentation.Validators.JobTitles;

public class EditJobTitleModelViewValidator : AbstractValidator<EditJobTitleModelView>
{
    public EditJobTitleModelViewValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("o Id é obrigatório.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("O Nome do cargo é obrigatorio.");

        RuleFor(x => x.DailyHours)
            .GreaterThan(TimeSpan.Zero)
            .WithMessage("A carga horária diária deve ser maior que zero.")
            .LessThanOrEqualTo(TimeSpan.FromHours(24))
            .WithMessage("A carga horária diária não pode ser maior que 24 horas.");
    }
}