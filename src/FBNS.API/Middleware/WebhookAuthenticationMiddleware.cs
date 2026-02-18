namespace FBNS.API.Middleware;

public class WebhookAuthenticationMiddleware(
    RequestDelegate next,
    IConfiguration configuration,
    ILogger<WebhookAuthenticationMiddleware> logger)
{
    private readonly string _expectedApiKey = configuration["Webhook:ApiKey"]
            ?? throw new InvalidOperationException("Webhook:ApiKey not configured");

    private readonly RequestDelegate _next = next;
    private readonly ILogger<WebhookAuthenticationMiddleware> _logger = logger;

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Path.StartsWithSegments("/health") ||
            context.Request.Path.StartsWithSegments("/swagger"))
        {
            await _next(context);
            return;
        }

        if (context.Request.Path.StartsWithSegments("/webhooks"))
        {
            if (!context.Request.Headers.TryGetValue("X-API-Key", out var apiKey))
            {
                _logger.LogWarning("Webhook request to {Path} rejected: Missing X-API-Key header", context.Request.Path);

                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(new
                {
                    error = "Unauthorized",
                    message = "X-API-Key header is required"
                });
                return;
            }

            if (apiKey != _expectedApiKey)
            {
                _logger.LogWarning("Webhook request to {Path} rejected: Invalid API key", context.Request.Path);

                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(new
                {
                    error = "Unauthorized",
                    message = "Invalid API key"
                });
                return;
            }

            _logger.LogDebug("Webhook request authenticated successfully");
        }

        await _next(context);
    }
}