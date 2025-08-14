using DailyQuizAPI.Features.Crosscutting.Users;
using DailyQuizAPI.Middlewares;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using System.Globalization;

namespace DailyQuizAPI.Mail;

public class SmtpEmailService(IOptions<SmtpOptions> options) : IEmailService
{
    private readonly SmtpOptions _options = options.Value;

    public async Task SendConfirmationLinkAsync(User user, string email, string confirmationLink, FrontEndNames frontEndName)
    {
        var subject = "Confirmation de ton adresse e-mail";
        var plainText = $"""
            Bonjour {user.UserName},

            Merci d'avoir créé ton compte ALED depuis notre application {Enum.GetName(frontEndName)}.
            Pour activer ton compte, clique sur le lien suivant :

            {confirmationLink}

            Si tu n’as pas demandé cela, ignore simplement ce message.
            """;

        var html = $"""
            <p>Bonjour {user.UserName},</p>
            <p>Merci d'avoir créé ton compte <strong>ALED</strong> depuis notre application <strong>{Enum.GetName(frontEndName)}</strong>.<br />
            Pour activer ton compte, clique sur le lien suivant :</p>
            <p><a href="{confirmationLink}">{confirmationLink}</a></p>
            <p>Si tu n’as pas demandé cela, ignore simplement ce message.</p>
            """;

        await SendEmailAsync(email, subject, plainText, html).ConfigureAwait(false);
    }

    public async Task SendPasswordResetLinkAsync(User user, string email, string resetLink, FrontEndNames frontEndName)
    {
        var subject = "Réinitialisation de ton mot de passe";
        var plainText = $"""
            Bonjour {user.UserName},

            Tu as initialisé une demande pour réinitialiser ton mot de passe depuis notre application 
            {Enum.GetName(frontEndName)}, pour cela, clique sur le lien suivant :

            {resetLink}

            Ce lien expirera sous peu.
            """;

        var html = $"""
            <p>Bonjour {user.UserName},</p>
            <p>Tu as initialisé une demande pour réinitialiser ton mot de passe depuis notre application 
            <strong>{Enum.GetName(frontEndName)}</strong>, pour cela, clique sur le lien suivant :</p>
            <p><a href="{resetLink}">{resetLink}</a></p>
            <p>Ce lien expirera sous peu.</p>
            """;

        await SendEmailAsync(email, subject, plainText, html).ConfigureAwait(false);
    }

    private async Task SendEmailAsync(string target, string subject, string plainTextContent, string? htmlContent = null)
    {
        using var email = new MimeMessage();
        email.From.Add(new MailboxAddress(_options.FromName, _options.FromEmail));
        email.To.Add(MailboxAddress.Parse(target));
        email.Subject = subject;

        var builder = new BodyBuilder
        {
            TextBody = plainTextContent,
            HtmlBody = htmlContent ?? plainTextContent
        };

        email.Body = builder.ToMessageBody();

        using var smtp = new SmtpClient();
        await smtp.ConnectAsync(_options.Host, _options.Port, MailKit.Security.SecureSocketOptions.StartTls).ConfigureAwait(false);
        await smtp.AuthenticateAsync(_options.UserName, _options.Password).ConfigureAwait(false);
        await smtp.SendAsync(email).ConfigureAwait(false);
        await smtp.DisconnectAsync(true).ConfigureAwait(false);
    }

    public async Task SendRollbackAsync(User user, string email, string rollbackLink, FrontEndNames frontEndName)
    {
        var subject = "Annulation des modifications de ton compte";
        var now = DateTime.Now.ToString("f", CultureInfo.GetCultureInfo("fr-FR"));

        var plainText = $"""
        Bonjour {user.UserName},

        Des modifications concernant l'e-mail de ton compte ALED ont été effectuées sur ton compte le {now}
        depuis notre application {frontEndName}.

        Si tu n’es pas à l’origine de ces changements, tu peux les annuler en cliquant sur le lien suivant :

        {rollbackLink}

        Si tu as bien effectué ces modifications, ignore simplement ce message.
        """;

        var html = $"""
        <p>Bonjour {user.UserName},</p>
        <p>Des modifications concernant l'e-mail de ton compte <strong>ALED</strong> ont été effectuées sur ton compte le <strong>{now}</strong>
        depuis notre application <strong>{frontEndName}</strong>.</p>
        <p>Si tu n’es pas à l’origine de ces changements, tu peux les annuler en cliquant sur le lien suivant :</p>
        <p><a href="{rollbackLink}">{rollbackLink}</a></p>
        <p>Si tu as bien effectué ces modifications, ignore simplement ce message.</p>
        """;

        await SendEmailAsync(email, subject, plainText, html).ConfigureAwait(false);
    }

    public async Task SendInactivityWarningAsync(User user, string email)
    {
        var subject = "Votre compte est inactif depuis 18 mois";
        var now = DateTime.Now.ToString("f", CultureInfo.GetCultureInfo("fr-FR"));

        var plainText = $"""
        Bonjour {user.UserName},

        Votre compte ALED est inactif depuis 18 mois.

        Conformément au Règlement Général sur la Protection des Données (RGPD), votre compte sera supprimé dans 6 mois si aucune connexion n’est effectuée d’ici là.

        Si vous souhaitez conserver votre compte, connectez-vous simplement avant cette échéance.

        Ce message a été généré automatiquement le {now}.
        """;

        var html = $"""
        <p>Bonjour {user.UserName},</p>
        <p>Votre compte <strong>ALED</strong> est inactif depuis 18 mois.</p>
        <p>Conformément au <strong>RGPD</strong>, votre compte sera supprimé dans 6 mois si aucune connexion n’est effectuée d’ici là.</p>
        <p>Si vous souhaitez conserver votre compte, connectez-vous simplement avant cette échéance.</p>
        <p><small>Ce message a été généré automatiquement le {now}.</small></p>
        """;

        await SendEmailAsync(email, subject, plainText, html).ConfigureAwait(false);
    }

    public async Task SendUserDeletedAsync(User user, string email)
    {
        var subject = "Suppression de votre compte";
        var now = DateTime.Now.ToString("f", CultureInfo.GetCultureInfo("fr-FR"));

        var plainText = $"""
        Bonjour {user.UserName},

        Votre compte ALED est inactif depuis 2 ans.

        Conformément au Règlement Général sur la Protection des Données (RGPD), votre compte et toutes vos données personnelles sont donc supprimés.

        Si vous souhaitez conserver votre compte, connectez-vous simplement avant cette échéance.

        Ce message a été généré automatiquement le {now}.
        """;

        var html = $"""
        <p>Bonjour {user.UserName},</p>
        <p>Votre compte <strong>ALED</strong> est inactif depuis 18 mois.</p>
        <p>Conformément au <strong>RGPD</strong>, votre compte sera supprimé dans 6 mois si aucune connexion n’est effectuée d’ici là.</p>
        <p>Si vous souhaitez conserver votre compte, connectez-vous simplement avant cette échéance.</p>
        <p><small>Ce message a été généré automatiquement le {now}.</small></p>
        """;

        await SendEmailAsync(email, subject, plainText, html).ConfigureAwait(false);
    }

    public async Task SendContactMessageAsync(string email, string name, string fromEmail, string message)
    {
        var subject = $"Nouveau message de contact de {name}";
        var plainText = $"""
            Nom: {name}
            Email: {fromEmail}

            Message:
            {message}
            """;

        var html = $"""
            <p><strong>Nom :</strong> {name}</p>
            <p><strong>Email :</strong> {fromEmail}</p>

            <p><strong>Message :</strong></p>
            <p>{message}</p>
            """;

        await SendEmailAsync(email, subject, plainText, html).ConfigureAwait(false);
    }
}
