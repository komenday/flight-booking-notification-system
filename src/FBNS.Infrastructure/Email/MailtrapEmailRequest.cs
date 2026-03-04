using System.Text.Json.Serialization;

namespace FBNS.Infrastructure.Email;

internal record MailtrapEmailRequest
{
    [JsonPropertyName("from")]
    public EmailAddress From { get; init; } = null!;

    [JsonPropertyName("to")]
    public EmailAddress[] To { get; init; } = [];

    [JsonPropertyName("subject")]
    public string Subject { get; init; } = string.Empty;

    [JsonPropertyName("html")]
    public string Html { get; init; } = string.Empty;

    [JsonPropertyName("text")]
    public string? Text { get; init; }
}