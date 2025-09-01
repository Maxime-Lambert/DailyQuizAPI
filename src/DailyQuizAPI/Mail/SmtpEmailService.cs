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

    private static string BuildButton(string url, string text) =>
        $"""
        <p style="margin: 20px 0; text-align: center;">
            <a href="{url}" style="
                display:inline-block;
                background:#4F46E5;
                color:white;
                padding:12px 24px;
                text-decoration:none;
                border-radius:8px;
                font-weight:bold;
                font-family:sans-serif;
            ">
                {text}
            </a>
        </p>
        """;

    private static string BuildTemplate(string userName, string content, string? footerNote = null)
    {
        var now = DateTime.Now.ToString("f", CultureInfo.GetCultureInfo("fr-FR"));

        return $"""
        <div style="max-width:600px;margin:auto;font-family:sans-serif;line-height:1.6;color:#333;">
            <div style="background:#4F46E5;color:white;padding:16px;border-radius:8px 8px 0 0;text-align:center;">
                <h2 style="margin:0;">ALED</h2>
            </div>
            <div style="padding:24px;background:white;border:1px solid #ddd;border-top:0;">
                <p>Salut {userName},</p>  
                {content}
                <p style="margin-top:32px;">À bientôt,<br>L’équipe ALED</p>
            </div>
            <div style="background:#f9f9f9;color:#666;padding:12px;font-size:12px;border-radius:0 0 8px 8px;text-align:center;">
                <p>Ce message a été généré automatiquement le {now}.</p>
                {(footerNote is not null ? $"<p>{footerNote}</p>" : "")}
            </div>
        </div>
        """;
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

    // --- Emails ---

    public async Task SendConfirmationLinkAsync(User user, string email, string confirmationLink, FrontEndNames frontEndName)
    {
        var subject = "Confirme ton adresse e-mail";
        var plainText = $"""
            Bonjour {user.UserName},

            Merci d'avoir créé ton compte ALED depuis notre application {frontEndName}.
            Pour activer ton compte, clique sur le lien suivant :

            {confirmationLink}

            Si tu n’as pas demandé cela, ignore simplement ce message.
            """;

        var html = BuildTemplate(user.UserName!, $"""
            <p>Merci d'avoir créé ton compte <strong>ALED</strong> depuis notre application <strong>{frontEndName}</strong>.</p>
            <p>Pour activer ton compte, clique sur le bouton ci-dessous :</p>
            {BuildButton(confirmationLink, "Activer mon compte")}
            <p>Si tu n’as pas demandé cela, ignore simplement ce message.</p>
        """);

        await SendEmailAsync(email, subject, plainText, html).ConfigureAwait(false);
    }

    public async Task SendPasswordResetLinkAsync(User user, string email, string resetLink, FrontEndNames frontEndName)
    {
        var subject = "Réinitialise ton mot de passe";
        var plainText = $"""
            Bonjour {user.UserName},

            Tu as demandé à réinitialiser ton mot de passe depuis {frontEndName}.
            Clique sur le lien suivant pour continuer :

            {resetLink}

            Ce lien expirera sous peu.
            """;

        var html = BuildTemplate(user.UserName!, $"""
            <p>Tu as demandé à réinitialiser ton mot de passe depuis <strong>{frontEndName}</strong>.</p>
            <p>Clique sur le bouton ci-dessous pour continuer :</p>
            {BuildButton(resetLink, "Réinitialiser mon mot de passe")}
            <p><small>Ce lien expirera sous peu.</small></p>
        """);

        await SendEmailAsync(email, subject, plainText, html).ConfigureAwait(false);
    }

    public async Task SendUsernameAsync(User user, string email, FrontEndNames frontEndName)
    {
        var subject = "Ton nom d’utilisateur";
        var plainText = $"""
            Bonjour,

            Tu as demandé à récupérer ton nom d'utilisateur depuis {frontEndName}.

            Ton nom d'utilisateur est : {user.UserName}
            """;

        var html = BuildTemplate(user.UserName!, $"""
            <p>Tu as demandé à récupérer ton nom d’utilisateur depuis <strong>{frontEndName}</strong>.</p>
            <p>Ton nom d’utilisateur est : <strong>{user.UserName}</strong></p>
        """);

        await SendEmailAsync(email, subject, plainText, html).ConfigureAwait(false);
    }

    public async Task SendRollbackAsync(User user, string email, string rollbackLink, FrontEndNames frontEndName)
    {
        var subject = "Annule les changements sur ton compte";
        var plainText = $"""
            Bonjour {user.UserName},

            Des modifications de l’e-mail de ton compte ALED ont été effectuées depuis {frontEndName}.
            Si tu n’es pas à l’origine de ces changements, annule-les ici :

            {rollbackLink}

            Sinon, ignore ce message.
            """;

        var html = BuildTemplate(user.UserName!, $"""
            <p>Des modifications de l’adresse e-mail de ton compte <strong>ALED</strong> ont été effectuées depuis <strong>{frontEndName}</strong>.</p>
            <p>Si tu n’es pas à l’origine de ces changements, tu peux les annuler en cliquant sur le bouton ci-dessous :</p>
            {BuildButton(rollbackLink, "Annuler les modifications")}
            <p>Si tu as bien effectué ces changements, ignore ce message.</p>
        """);

        await SendEmailAsync(email, subject, plainText, html).ConfigureAwait(false);
    }

    public async Task SendInactivityWarningAsync(User user, string email)
    {
        var subject = "Ton compte est inactif depuis 18 mois";
        var plainText = $"""
            Bonjour {user.UserName},

            Ton compte ALED est inactif depuis 18 mois.
            Conformément au RGPD, il sera supprimé dans 6 mois si tu ne te reconnectes pas.

            Connecte-toi avant cette échéance pour conserver ton compte.
            """;

        var html = BuildTemplate(user.UserName!, $"""
            <p>Ton compte <strong>ALED</strong> est inactif depuis 18 mois.</p>
            <p>Conformément au <strong>RGPD</strong>, il sera supprimé dans 6 mois si tu ne te reconnectes pas.</p>
            <p><strong>Connecte-toi avant cette échéance pour conserver ton compte.</strong></p>
        """);

        await SendEmailAsync(email, subject, plainText, html).ConfigureAwait(false);
    }

    public async Task SendUserDeletedAsync(User user, string email)
    {
        var subject = "Ton compte a été supprimé";
        var plainText = $"""
            Bonjour {user.UserName},

            Ton compte ALED est resté inactif pendant 2 ans et a donc été supprimé, conformément au RGPD.

            Toutes tes données personnelles ont été effacées.
            """;

        var html = BuildTemplate(user.UserName!, $"""
            <p>Ton compte <strong>ALED</strong> est resté inactif pendant <strong>2 ans</strong> et a donc été supprimé, conformément au <strong>RGPD</strong>.</p>
            <p>Toutes tes données personnelles ont été effacées.</p>
        """);

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
            <div style="font-family:sans-serif;max-width:600px;margin:auto;">
                <p><strong>Nom :</strong> {name}</p>
                <p><strong>Email :</strong> {fromEmail}</p>
                <p><strong>Message :</strong></p>
                <blockquote style="border-left:3px solid #ddd;padding-left:12px;color:#555;">{message}</blockquote>
            </div>
        """;

        await SendEmailAsync(email, subject, plainText, html).ConfigureAwait(false);
    }
}
