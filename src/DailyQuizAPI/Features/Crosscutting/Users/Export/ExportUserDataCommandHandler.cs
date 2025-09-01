using DailyQuizAPI.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace DailyQuizAPI.Features.Crosscutting.Users.Export;

public sealed class ExportUserDataCommandHandler(QuizContext quizContext)
{
    private static readonly JsonSerializerOptions CACHED_JSON_SERIALIZER_OPTIONS = new() { WriteIndented = true };
    private readonly QuizContext _quizContext = quizContext;

    public async Task<ExportUserDataResponse> Handle(ClaimsPrincipal claims)
    {
        var userId = claims.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new InvalidOperationException("Connexion invalide");

        var user = await _quizContext.Users
            .Include(u => u.SumotHistories)
            .FirstOrDefaultAsync(u => u.Id == userId)
            .ConfigureAwait(false)
            ?? throw new InvalidOperationException("Connexion invalide");

        var exportObject = new
        {
            user.Id,
            user.UserName,
            user.Email,
            user.KeyboardLayout,
            user.ColorblindMode,
            user.SmartKeyboardType,
            user.PlaysWithDifficultWords,
            user.LastLogin,
            user.SumotHistories
        };

        var json = JsonSerializer.Serialize(exportObject, CACHED_JSON_SERIALIZER_OPTIONS);
        var bytes = Encoding.UTF8.GetBytes(json);

        return new(bytes, $"userdata-{user.UserName}.json", "application/json");
    }
}


