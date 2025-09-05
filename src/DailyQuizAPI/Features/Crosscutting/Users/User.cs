using DailyQuizAPI.Features.SumotApp.SumotHistories;
using Microsoft.AspNetCore.Identity;

namespace DailyQuizAPI.Features.Crosscutting.Users;

public sealed class User : IdentityUser
{
    public KeyboardLayout KeyboardLayout { get; set; } = KeyboardLayout.AZERTY;

    public ColorblindMode ColorblindMode { get; set; } = ColorblindMode.None;

    public SmartKeyboardType SmartKeyboardType { get; set; } = SmartKeyboardType.Correct;

    public bool PlaysWithDifficultWords { get; set; }

    public ICollection<RefreshToken> RefreshTokens { get; } = [];

    public ICollection<SumotHistory> SumotHistories { get; private set; } = [];

    public DateOnly? LastLogin { get; set; }

    public void AddHistory(SumotHistory attempt) => SumotHistories.Add(attempt);

    public void ClearHistory() => SumotHistories.Clear();
}

