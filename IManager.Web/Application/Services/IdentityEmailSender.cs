using IManager.Web.Application.Interfaces;
using IManager.Web.Domain.Entities.Users;
using Microsoft.AspNetCore.Identity;

namespace IManager.Web.Application.Services;

public class IdentityEmailSender : IEmailSender<User>
{
    private readonly IEmailService _emailService;

    public IdentityEmailSender(IEmailService emailService)
    {
        _emailService = emailService;
    }

    public Task SendConfirmationLinkAsync(User user, string email, string confirmationLink)
    {
        var body = $"<p>Olá! Confirme seu e-mail clicando no link abaixo:</p><a href='{confirmationLink}'>Confirmar e-mail</a>";
        return _emailService.SendEmailAsync(email, user.UserName ?? email, "Confirme seu e-mail", body);
    }

    public Task SendPasswordResetLinkAsync(User user, string email, string resetLink)
    {
        var body = $"<p>Olá! Redefina sua senha clicando no link abaixo:</p><a href='{resetLink}'>Redefinir senha</a>";
        return _emailService.SendEmailAsync(email, user.UserName ?? email, "Redefinir senha", body);
    }

    public Task SendPasswordResetCodeAsync(User user, string email, string resetCode)
    {
        var body = $"<p>Olá! Seu código de redefinição de senha é:</p><h2>{resetCode}</h2>";
        return _emailService.SendEmailAsync(email, user.UserName ?? email, "Código de redefinição", body);
    }
}
