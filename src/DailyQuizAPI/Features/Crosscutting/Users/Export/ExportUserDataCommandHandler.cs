using DailyQuizAPI.Common.Exceptions;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Text.Json;

namespace DailyQuizAPI.Features.Crosscutting.Users.Export;

public sealed class ExportUserDataCommandHandler(UserManager<User> userManager)
{
    private static readonly JsonSerializerOptions CACHED_JSON_SERIALIZER_OPTIONS = new() { WriteIndented = true };
    private readonly UserManager<User> _userManager = userManager;

    public async Task<ExportUserDataResponse> Handle(ClaimsPrincipal claims)
    {
        var userId = claims.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new NotFoundException("Utilisateur introuvable dans les revendications.");

        var user = await _userManager.FindByIdAsync(userId).ConfigureAwait(false)
            ?? throw new InvalidOperationException("L'utilisateur connecté est introuvable.");

        var exportObject = new
        {
            user.Id,
            user.UserName,
            user.Email,
            user.KeyboardLayout,
            user.ColorblindMode,
            user.SmartKeyboardType,
            user.LastLogin,
            RefreshTokens = user.RefreshTokens.Select(rt => new { rt.Token, rt.ExpiresAt }),
            user.SumotHistories
        };

        var json = JsonSerializer.Serialize(exportObject, CACHED_JSON_SERIALIZER_OPTIONS);
        var bytes = System.Text.Encoding.UTF8.GetBytes(json);

        return new ExportUserDataResponse(bytes, $"userdata-{user.UserName}.json", "application/json");
    }
}

