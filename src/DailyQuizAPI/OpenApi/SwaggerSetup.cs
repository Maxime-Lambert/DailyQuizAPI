using DailyQuizAPI.Middlewares.Authentication;
using Microsoft.OpenApi.Models;

namespace DailyQuizAPI.OpenApi;

public static class SwaggerSetup
{
    private const string JWT_FORMAT = "JWT";
    private const string BEARER_HEADER_NAME = "Authorization";
    private const string BEARER_DESCRIPTION = "Entrez votre token JWT au format **Bearer &lt;token&gt;**";
    private const string SWAGGER_DARK_CSS_PATH = "/swagger-ui/SwaggerDark.css";

    public static IServiceCollection AddCustomSwagger(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(options =>
        {
            options.EnableAnnotations();

            options.AddSecurityDefinition(AuthSchemes.JWT, new OpenApiSecurityScheme
            {
                Name = BEARER_HEADER_NAME,
                Type = SecuritySchemeType.Http,
                Scheme = AuthSchemes.JWT,
                BearerFormat = JWT_FORMAT,
                In = ParameterLocation.Header,
                Description = BEARER_DESCRIPTION
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                { new OpenApiSecurityScheme { Reference = new() { Type = ReferenceType.SecurityScheme, Id = AuthSchemes.JWT } }, Array.Empty<string>() }
            });
        });

        return services;
    }

    public static WebApplication UseSwaggerDark(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.InjectStylesheet(SWAGGER_DARK_CSS_PATH);
        });
        return app;
    }
}


