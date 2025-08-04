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

    [LoggerMessage(EventId = 1005, Level = LogLevel.Information,
        Message = "{Count} users deleted for inactivity : {Day}")]
    public static partial void LogUsersDeleted(
        this ILogger logger,
        int count,
        DateOnly day);

    [LoggerMessage(EventId = 1006, Level = LogLevel.Information,
        Message = "Avertissement envoyé à {Email} : {Day}")]
    public static partial void LogInactivityUserWarning(
        this ILogger logger,
        string email,
        DateOnly day);

    [LoggerMessage(EventId = 1007, Level = LogLevel.Information,
        Message = "Utilisateur supprimé envoyé à {Email} : {Day}")]
    public static partial void LogUserDeleted(
        this ILogger logger,
        string email,
        DateOnly day);

    [LoggerMessage(EventId = 1008, Level = LogLevel.Warning,
        Message = "Erreur lors de l'envoi du mail à {Email} : {ExceptionMessage}")]
    public static partial void LogEmailSendException(
        this ILogger logger,
        string email,
        string exceptionMessage);
}
