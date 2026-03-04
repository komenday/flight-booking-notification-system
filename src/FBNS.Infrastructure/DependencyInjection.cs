using FBNS.Application.Services;
using FBNS.Infrastructure.Email;
using FBNS.Infrastructure.Logging;
using FBNS.Infrastructure.OptionsValidator;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Polly;
using Polly.CircuitBreaker;
using Polly.Extensions.Http;
using Polly.Retry;

namespace FBNS.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<MailtrapApiOptions>(
            configuration.GetSection(MailtrapApiOptions.SectionName));

        services.AddSingleton<IValidateOptions<MailtrapApiOptions>, MailtrapApiOptionsValidator>();

        services.AddHttpClient<MailtrapApiEmailService>((serviceProvider, client) =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<MailtrapApiOptions>>().Value;

            client.BaseAddress = new Uri("https://sandbox.api.mailtrap.io/");

            var token = options.ApiToken.StartsWith("Bearer ")
                ? options.ApiToken[7..]
                : options.ApiToken;

            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            client.Timeout = TimeSpan.FromSeconds(30);
        })
        .AddPolicyHandler(GetCircuitBreakerPolicy())
        .AddPolicyHandler(GetRetryPolicy());

        services.AddScoped<IEmailService>(serviceProvider =>
            serviceProvider.GetRequiredService<MailtrapApiEmailService>());

        services.Configure<FileLoggerOptions>(configuration.GetSection(FileLoggerOptions.SectionName));

        services.AddSingleton<IFileNotificationLogger, FileNotificationLogger>();

        return services;
    }

    private static AsyncCircuitBreakerPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .AdvancedCircuitBreakerAsync(
                failureThreshold: 0.5,
                samplingDuration: TimeSpan.FromSeconds(60),
                minimumThroughput: 3,                   
                durationOfBreak: TimeSpan.FromSeconds(5)
            );
    }

    private static AsyncRetryPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt))
            );
    }
}