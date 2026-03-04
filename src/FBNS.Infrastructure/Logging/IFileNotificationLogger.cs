namespace FBNS.Infrastructure.Logging;

public interface IFileNotificationLogger
{
    Task LogNotificationSentAsync(NotificationLog log);

    Task LogNotificationFailedAsync(string to, string subject, string error);
}
