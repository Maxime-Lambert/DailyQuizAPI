namespace DailyQuizAPI.Middlewares;

public static partial class LoggerMessages
{
    [LoggerMessage(EventId = 1000, Level = LogLevel.Error,
        Message = "Unhandled exception: {ExceptionMessage}")]
    public static partial void LogUnhandledException(
        this ILogger logger,
        string exceptionMessage,
        Exception exception);

    [LoggerMessage(EventId = 1001, Level = LogLevel.Error,
        Message = "Invalid Operation Exception : {ExceptionMessage}")]
    public static partial void LogInvalidOperationException(
        this ILogger logger,
        string exceptionMessage,
        Exception exception);

    [LoggerMessage(EventId = 1002, Level = LogLevel.Warning,
        Message = "Request to {Path} took {ElapsedMilliseconds}ms")]
    public static partial void LogRequestTiming(
        this ILogger logger,
        string path,
        long elapsedMilliseconds);

    [LoggerMessage(EventId = 1003, Level = LogLevel.Warning,
        Message = "No sumots can be chosen for today {Day}")]
    public static partial void LogNoSumotPossible(
        this ILogger logger,
        DateOnly day);

    [LoggerMessage(EventId = 1004, Level = LogLevel.Information,
        Message = "Sumot chosen {Word} for {Day}")]
    public static partial void LogSumotChosen(
        this ILogger logger,
        string word,
        DateOnly day);
}
