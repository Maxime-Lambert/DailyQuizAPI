using DailyQuizAPI.Persistence;

namespace DailyQuizAPI.AppSettings.CreateAppSetting;

public sealed class CreateAppSettingCommandHandler(QuizContext quizContext)
{
    private readonly QuizContext _quizContext = quizContext;

    public async Task Handle(CreateAppSettingCommand request, CancellationToken cancellationToken)
    {
        var appSetting = new AppSetting
        {
            Key = request.Key,
            Value = request.Value
        };
        _quizContext.AppSettings.Add(appSetting);
        await _quizContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}
