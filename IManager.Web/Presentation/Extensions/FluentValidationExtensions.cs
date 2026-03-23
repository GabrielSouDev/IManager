using FluentValidation;
using FluentValidation.AspNetCore;

namespace IManager.Web.Presentation.Extensions;

public static class FluentValidationExtensions
{
    public static void AddFluentValidation(this WebApplicationBuilder builder)
    {
        builder.Services.AddFluentValidationAutoValidation();

        builder.Services.AddFluentValidationClientsideAdapters();

        builder.Services.AddValidatorsFromAssemblyContaining<Program>();
    }
}
