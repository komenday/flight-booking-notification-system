using System.Text.Json.Serialization;

namespace FBNS.Infrastructure.Email;

internal record EmailAddress
{
    [JsonPropertyName("email")]
    public string Email { get; init; } = string.Empty;

    [JsonPropertyName("name")]
    public string? Name { get; init; }
}