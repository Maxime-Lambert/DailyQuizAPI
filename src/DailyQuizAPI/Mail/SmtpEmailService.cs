using DailyQuizAPI.Features.Crosscutting.Users;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using System.Globalization;

namespace DailyQuizAPI.Mail;

public class SmtpEmailService(IOptions<SmtpOptions> options) : IEmailService
{
    private readonly SmtpOptions _options = options.Value;

    public async Task SendConfirmationLinkAsync(User user, string email, string confirmationLink)
    {
        var subject = "Confirmation de ton adresse e-mail";
        var plainText = $"""
            Bonjour {user.UserName},

            Merci d'avoir créé ton compte ALED.
            Pour activer ton compte, clique sur le lien suivant :

            {confirmationLink}

            Si tu n’as pas demandé cela, ignore simplement ce message.
            """;

        var html = $"""
            <p>Bonjour {user.UserName},</p>
            <p>Merci d'avoir créé ton compte <strong>ALED</strong>.<br />
            Pour activer ton compte, clique sur le lien suivant :</p>
            <p><a href="{confirmationLink}">{confirmationLink}</a></p>
            <p>Si tu n’as pas demandé cela, ignore simplement ce message.</p>
            """;

        await SendEmailAsync(email, subject, plainText, html).ConfigureAwait(false);
    }

    public async Task SendPasswordResetCodeAsync(User user, string email, string resetCode)
    {
        var subject = "Code de réinitialisation de ton mot de passe";
        var plainText = $"""
            Bonjour {user.UserName},

            Voici ton code pour réinitialiser ton mot de passe : {resetCode}

            Ce code est valide pour une durée limitée.
            """;

        var html = $"""
            <p>Bonjour {user.UserName},</p>
            <p>Voici ton code de réinitialisation :</p>
            <h2>{resetCode}</h2>
            <p>Ce code est valide pour une durée limitée.</p>
            """;

        await SendEmailAsync(email, subject, plainText, html).ConfigureAwait(false);
    }

    public async Task SendPasswordResetLinkAsync(User user, string email, string resetLink)
    {
        var subject = "Réinitialisation de ton mot de passe";
        var plainText = $"""
            Bonjour {user.UserName},

            Pour réinitialiser ton mot de passe, clique sur le lien suivant :

            {resetLink}

            Ce lien expirera sous peu.
            """;

        var html = $"""
            <p>Bonjour {user.UserName},</p>
            <p>Pour réinitialiser ton mot de passe, clique sur le lien suivant :</p>
            <p><a href="{resetLink}">{resetLink}</a></p>
            <p>Ce lien expirera sous peu.</p>
            """;

        await SendEmailAsync(email, subject, plainText, html).ConfigureAwait(false);
    }

    public async Task SendEmailAsync(string target, string subject, string plainTextContent, string? htmlContent = null)
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

    public async Task SendRollbackAsync(User user, string email, string rollbackLink)
    {
        var subject = "Annulation des modifications de ton compte";
        var now = DateTime.Now.ToString("f", CultureInfo.GetCultureInfo("fr-FR"));

        var plainText = $"""
        Bonjour {user.UserName},

        Des modifications concernant ton e-mail ont été effectuées sur ton compte le {now}.

        Si tu n’es pas à l’origine de ces changements, tu peux les annuler en cliquant sur le lien suivant :

        {rollbackLink}

        Si tu as bien effectué ces modifications, ignore simplement ce message.
        """;

        var html = $"""
        <p>Bonjour {user.UserName},</p>
        <p>Des modifications concernant ton e-mail ont été effectuées sur ton compte le <strong>{now}</strong>.</p>
        <p>Si tu n’es pas à l’origine de ces changements, tu peux les annuler en cliquant sur le lien suivant :</p>
        <p><a href="{rollbackLink}">{rollbackLink}</a></p>
        <p>Si tu as bien effectué ces modifications, ignore simplement ce message.</p>
        """;

        await SendEmailAsync(email, subject, plainText, html).ConfigureAwait(false);
    }
    public async Task SendInactivityWarningAsync(User user, string email)
    {
        var subject = "Votre compte est inactif depuis 18 mois";
        var now = DateTime.Now.ToString("f", CultureInfo.GetCultureInfo("fr-FR")); // ex : mardi 29 juillet 2025 21:42

        var plainText = $"""
        Bonjour {user.UserName},

        Votre compte est inactif depuis plus de 18 mois.

        Conformément au Règlement Général sur la Protection des Données (RGPD), votre compte sera supprimé dans 6 mois si aucune connexion n’est effectuée d’ici là.

        Si vous souhaitez conserver votre compte, connectez-vous simplement avant cette échéance.

        Ce message a été généré automatiquement le {now}.
        """;

        var html = $"""
        <p>Bonjour {user.UserName},</p>
        <p>Votre compte est inactif depuis plus de 18 mois.</p>
        <p>Conformément au <strong>RGPD</strong>, votre compte sera supprimé dans 6 mois si aucune connexion n’est effectuée d’ici là.</p>
        <p>Si vous souhaitez conserver votre compte, connectez-vous simplement avant cette échéance.</p>
        <p><small>Ce message a été généré automatiquement le {now}.</small></p>
        """;

        await SendEmailAsync(email, subject, plainText, html).ConfigureAwait(false);
    }
}
