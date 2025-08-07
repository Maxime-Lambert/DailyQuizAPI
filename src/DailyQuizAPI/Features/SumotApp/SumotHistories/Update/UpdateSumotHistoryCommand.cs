using System.Collections.ObjectModel;

namespace DailyQuizAPI.Features.SumotApp.SumotHistories.Update;

public sealed record UpdateSumotHistoryCommand(string Word, Collection<string> Tries);
