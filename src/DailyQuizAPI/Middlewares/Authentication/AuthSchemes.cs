using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace DailyQuizAPI.Middlewares.Authentication;

public static class AuthSchemes
{
    public const string JWT = JwtBearerDefaults.AuthenticationScheme;
}