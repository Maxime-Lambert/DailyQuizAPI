namespace DailyQuizAPI.Features.Crosscutting.Users.Export;

public sealed record ExportUserDataResponse(IReadOnlyList<byte> FileContent, string FileName, string ContentType);