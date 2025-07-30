using DailyQuizAPI.Middlewares.Authentication.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace DailyQuizAPI.Middlewares.Authentication;

public static class AuthenticationSetup
{
    public static IServiceCollection AddCustomAuthentication(this IServiceCollection services, IConfiguration config)
    {
        services.ConfigureOptions<AuthenticationOptionsSetup>();

        services.AddAuthentication()
            .AddJwtBearer(AuthSchemes.JWT, options =>
            {
                var serviceProvider = services.BuildServiceProvider();
                var authOptions = serviceProvider.GetRequiredService<IOptions<AuthenticationOptions>>().Value;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = authOptions.Issuer,
                    ValidAudience = authOptions.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(authOptions.Secret))
                };
            });

        return services;
    }
}

