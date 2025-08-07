using System.Collections.ObjectModel;

namespace DailyQuizAPI.Features.SumotApp.SumotHistories.Update;

public sealed record UpdateSumotHistoriesCommand(Collection<UpdateSumotHistoryCommand> Histories);