using Microsoft.AspNetCore.Identity;

namespace DailyQuizAPI.Users;

public sealed class User : IdentityUser
{
    private readonly List<SumotHistory> _sumotHistories = [];
    public IReadOnlyCollection<SumotHistory> SumotHistories => _sumotHistories.AsReadOnly();

    public void AddHistory(SumotHistory attempt) => _sumotHistories.Add(attempt);
    public void ClearHistory() => _sumotHistories.Clear();
}

