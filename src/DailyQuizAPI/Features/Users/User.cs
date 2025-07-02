using DailyQuizAPI.Features.SumotHistories;
using Microsoft.AspNetCore.Identity;

namespace DailyQuizAPI.Features.Users;

public sealed class User : IdentityUser
{
    private readonly List<SumotHistory> _sumotHistories = [];

    public TypeClavier TypeClavier { get; set; } = TypeClavier.AZERTY;

    public ModeDaltonien ModeDaltonien { get; set; } = ModeDaltonien.None;

    public ICollection<RefreshToken> RefreshTokens
    { get; } = [];

    public IReadOnlyCollection<SumotHistory> SumotHistories => _sumotHistories.AsReadOnly();

    public void AddHistory(SumotHistory attempt) => _sumotHistories.Add(attempt);

    public void ClearHistory() => _sumotHistories.Clear();
}

