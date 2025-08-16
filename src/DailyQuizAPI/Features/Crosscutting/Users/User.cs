using DailyQuizAPI.Features.SumotApp.SumotHistories;
using Microsoft.AspNetCore.Identity;

namespace DailyQuizAPI.Features.Crosscutting.Users;

public sealed class User : IdentityUser
{
    private readonly List<SumotHistory> _sumotHistories = [];

    public KeyboardLayout KeyboardLayout { get; set; } = KeyboardLayout.AZERTY;

    public ColorblindMode ColorblindMode { get; set; } = ColorblindMode.None;

    public SmartKeyboardType SmartKeyboardType { get; set; } = SmartKeyboardType.Correct;

    public bool PlaysWithDifficultWords { get; set; }

    public ICollection<RefreshToken> RefreshTokens { get; } = [];

    public IReadOnlyCollection<SumotHistory> SumotHistories => _sumotHistories.AsReadOnly();

    public DateOnly? LastLogin { get; set; }

    public void AddHistory(SumotHistory attempt) => _sumotHistories.Add(attempt);

    public void ClearHistory() => _sumotHistories.Clear();
}

