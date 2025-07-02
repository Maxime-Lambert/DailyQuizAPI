using System.Collections.ObjectModel;

namespace DailyQuizAPI.Features.SumotHistories.Add;

public sealed record AddSumotHistoryCommand(string Word, Collection<string> Tries, DateOnly Day);

