using System.Text.Json.Serialization;

namespace FBNS.Infrastructure.Email;

internal record MailtrapApiResponse
{
    [JsonPropertyName("success")]
    public bool Success { get; init; }

    [JsonPropertyName("message_ids")]
    public string[] MessageIds { get; init; } = [];
}