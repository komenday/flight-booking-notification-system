using FBNS.Application.Services;
using FBNS.Infrastructure.Email;
using FBNS.Infrastructure.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FBNS.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<SendGridOptions>(configuration.GetSection(SendGridOptions.SectionName));
        services.Configure<FileLoggerOptions>(configuration.GetSection(FileLoggerOptions.SectionName));

        services.AddSingleton<IFileNotificationLogger, FileNotificationLogger>();
        services.AddScoped<IEmailService, SendGridEmailService>();

        return services;
    }
}