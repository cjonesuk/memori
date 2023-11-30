using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Memori.Processing;

public static class Extensions
{
    public static IServiceCollection AddProcessingManager(this IServiceCollection services)
    {
        services.AddSingleton<ProcessingManagerBackgroundService>();
        services.AddSingleton<IHostedService, ProcessingManagerBackgroundService>(
                   serviceProvider => serviceProvider.GetRequiredService<ProcessingManagerBackgroundService>());

        services.AddTransient<ProcessingJob>();

        return services;
    }
}
