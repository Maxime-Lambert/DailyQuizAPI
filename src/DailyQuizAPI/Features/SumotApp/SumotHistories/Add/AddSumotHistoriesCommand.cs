using System.Collections.ObjectModel;

namespace DailyQuizAPI.Features.SumotApp.SumotHistories.Add;

public sealed record AddSumotHistoriesCommand(Collection<AddSumotHistoryCommand> Histories);

