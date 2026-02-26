using FBNS.Infrastructure.Email;
using Microsoft.Extensions.Options;

namespace FBNS.Infrastructure.OptionsValidator;

public class MailtrapApiOptionsValidator : IValidateOptions<MailtrapApiOptions>
{
    public ValidateOptionsResult Validate(string? name, MailtrapApiOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.ApiToken))
        {
            return ValidateOptionsResult.Fail(
                "MailtrapApi:ApiToken is required. " +
                "Get it from: https://mailtrap.io/api-tokens"
            );
        }

        var token = options.ApiToken.StartsWith("Bearer ")
            ? options.ApiToken[7..].Trim()
            : options.ApiToken.Trim();

        if (token.Length < 20)
        {
            return ValidateOptionsResult.Fail(
                "MailtrapApi:ApiToken format is invalid. Token is too short.");
        }

        if (string.IsNullOrWhiteSpace(options.InboxId))
        {
            return ValidateOptionsResult.Fail(
                "MailtrapApi:InboxId is required. " +
                "Get it from your Mailtrap inbox URL: https://mailtrap.io/inboxes/{INBOX_ID}");
        }

        if (!long.TryParse(options.InboxId, out _))
        {
            return ValidateOptionsResult.Fail(
                $"MailtrapApi:InboxId '{options.InboxId}' is not a valid number. " +
                "It should be numeric (e.g., '123456' or '2508389').");
        }

        if (string.IsNullOrWhiteSpace(options.FromEmail))
        {
            return ValidateOptionsResult.Fail("MailtrapApi:FromEmail is required.");
        }

        if (!options.FromEmail.Contains('@'))
        {
            return ValidateOptionsResult.Fail(
                $"MailtrapApi:FromEmail '{options.FromEmail}' is not a valid email address.");
        }

        return ValidateOptionsResult.Success;
    }
}