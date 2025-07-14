using System.Collections.ObjectModel;

namespace DailyQuizAPI.Features.SumotApp.SumotHistories.Add;

public sealed record AddSumotHistoryCommand(string Word, Collection<string> Tries);

