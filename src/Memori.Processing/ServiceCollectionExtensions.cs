using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Memori.Processing;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddJobManager(this IServiceCollection services)
    {
        services.AddSingleton<JobManager>();
        services.AddSingleton<IJobManager>(serviceProvider => serviceProvider.GetRequiredService<JobManager>());
        services.AddSingleton<IHostedService, JobManager>(
                   serviceProvider => serviceProvider.GetRequiredService<JobManager>());


        return services;
    }
}
