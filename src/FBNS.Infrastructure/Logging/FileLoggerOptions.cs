namespace FBNS.Infrastructure.Logging;

public class FileLoggerOptions
{
    public const string SectionName = "FileLogger";

    public string LogFilePath { get; set; } = string.Empty;
}