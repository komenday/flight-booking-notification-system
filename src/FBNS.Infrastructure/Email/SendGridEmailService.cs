using FBNS.Application.Services;
using FBNS.Infrastructure.Logging;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace FBNS.Infrastructure.Email;

public class SendGridEmailService : IEmailService
{
    private readonly SendGridClient _client;
    private readonly ILogger<SendGridEmailService> _logger;
    private readonly IFileNotificationLogger _fileLogger;

    private readonly string _fromEmail;
    private readonly string _fromName;

    public SendGridEmailService(
        IOptions<SendGridOptions> options,
        ILogger<SendGridEmailService> logger,
        IFileNotificationLogger fileLogger)
    {
        var sendGridOptions = options.Value;

        if (string.IsNullOrWhiteSpace(sendGridOptions.ApiKey))
        {
            throw new InvalidOperationException("SendGrid API key is not configured");
        }

        _client = new SendGridClient(sendGridOptions.ApiKey);
        _fromEmail = sendGridOptions.FromEmail;
        _fromName = sendGridOptions.FromName;
        _logger = logger;
        _fileLogger = fileLogger;
    }

    public async Task SendAsync(string to, string subject, string htmlContent, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Sending email to {To} with subject '{Subject}'", to, subject);

        var message = new SendGridMessage
        {
            From = new EmailAddress(_fromEmail, _fromName),
            Subject = subject,
            HtmlContent = htmlContent
        };
        message.AddTo(new EmailAddress(to));
        message.SetClickTracking(false, false);

        try
        {
            var response = await _client.SendEmailAsync(message, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Email sent successfully to {To}. Status: {StatusCode}", to, response.StatusCode);

                await _fileLogger.LogNotificationSentAsync(new NotificationLog
                {
                    To = to,
                    Subject = subject,
                    SentAt = DateTime.UtcNow,
                    Status = "Success",
                    Provider = "SendGrid"
                });
            }
            else
            {
                var responseBody = await response.Body.ReadAsStringAsync(cancellationToken);

                _logger.LogError("Failed to send email to {To}. Status: {StatusCode}, Response: {Response}", to, response.StatusCode, responseBody);

                await _fileLogger.LogNotificationFailedAsync(to, subject, $"SendGrid returned {response.StatusCode}: {responseBody}");

                throw new InvalidOperationException($"SendGrid API returned {response.StatusCode}. Response: {responseBody}");
            }
        }
        catch (Exception ex) when (ex is not InvalidOperationException)
        {
            _logger.LogError(ex, "Exception occurred while sending email to {To}", to);

            await _fileLogger.LogNotificationFailedAsync(to, subject, ex.Message);
            throw;
        }
    }
}