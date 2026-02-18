using FBNS.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace FBNS.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<INotificationService, NotificationService>();

        return services;
    }
}
