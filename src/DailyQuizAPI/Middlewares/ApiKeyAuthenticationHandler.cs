using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace DailyQuizAPI.Middlewares;

public sealed class ApiKeyAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly string _expectedKey;

    public ApiKeyAuthenticationHandler(
        IOptions<Authentication.Options.AuthenticationOptions> apiKeyOptions,
        IOptionsMonitor<AuthenticationSchemeOptions> schemeOptions,
        ILoggerFactory loggerFactory,
        UrlEncoder encoder)
        : base(schemeOptions, loggerFactory, encoder)
    {
        _expectedKey = apiKeyOptions.Value.ApiKey;
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue("API-KEY", out var providedKey))
            return Task.FromResult(AuthenticateResult.Fail("Missing API-KEY"));

        if (providedKey != _expectedKey)
            return Task.FromResult(AuthenticateResult.Fail("Invalid API Key"));

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, "ApiClient"),
            new Claim("AuthType", "ApiKey")
        };

        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}


