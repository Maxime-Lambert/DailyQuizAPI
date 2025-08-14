namespace DailyQuizAPI.Common.Exceptions;

public sealed class NotFoundException : Exception
{
    public NotFoundException()
    {
    }

    public NotFoundException(string message)
        : base(message)
    {
    }

    public NotFoundException(string name, object key)
        : base($"L'entité {name} ({key}) n'a pas pu être trouvée")
    {
    }

    public NotFoundException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
