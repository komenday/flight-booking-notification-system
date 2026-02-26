using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FBNS.Infrastructure.Logging;

public class FileNotificationLogger : IFileNotificationLogger
{
    private readonly string _logFilePath;
    private readonly ILogger<FileNotificationLogger> _logger;

    private readonly SemaphoreSlim _semaphore = new(1, 1);

    public FileNotificationLogger(IOptions<FileLoggerOptions> options, ILogger<FileNotificationLogger> logger)
    {
        _logFilePath = options.Value.LogFilePath;
        _logger = logger;
        EnsureLogFileExists();
    }

    public async Task LogNotificationSentAsync(NotificationLog log)
    {
        var logEntry = string.Format(
            "[{0:yyyy-MM-dd HH:mm:ss}] SUCCESS | To: {1} | Subject: {2} | Provider: {3}",
            log.SentAt,
            log.To,
            log.Subject,
            log.Provider
        );

        await WriteToFileAsync(logEntry);
    }

    public async Task LogNotificationFailedAsync(string to, string subject, string error)
    {
        var logEntry = string.Format(
            "[{0:yyyy-MM-dd HH:mm:ss}] FAILED | To: {1} | Subject: {2} | Error: {3}",
            DateTime.UtcNow,
            to,
            subject,
            error
        );

        await WriteToFileAsync(logEntry);
    }

    private async Task WriteToFileAsync(string logEntry)
    {
        await _semaphore.WaitAsync();
        try
        {
            var directory = Path.GetDirectoryName(_logFilePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            await File.AppendAllLinesAsync(_logFilePath, [logEntry]);

            _logger.LogDebug("Logged notification to file: {LogEntry}", logEntry);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to write to notification log file");
        }
        finally
        {
            _semaphore.Release();
        }
    }

    private void EnsureLogFileExists()
    {
        try
        {
            var directory = Path.GetDirectoryName(_logFilePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            if (!File.Exists(_logFilePath))
            {
                File.WriteAllText(_logFilePath, $"# Notification Log - Started at {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC{Environment.NewLine}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize notification log file");
        }
    }
}