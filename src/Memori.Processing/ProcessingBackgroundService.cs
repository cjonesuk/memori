using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading.Channels;

namespace Memori.Processing;

public sealed class ProcessingManagerBackgroundService : BackgroundService
{
    private readonly ILogger _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly Channel<IProcessingJobDescription> _requests;

    public ProcessingManagerBackgroundService(ILogger<ProcessingManagerBackgroundService> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _requests = Channel.CreateUnbounded<IProcessingJobDescription>();
    }

    public bool RequestJob(IProcessingJobDescription jobDescription)
    {
        return _requests.Writer.TryWrite(jobDescription);
    }


    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Processing background service is running.");

        await foreach (var request in _requests.Reader.ReadAllAsync(stoppingToken))
        {
            try
            {
                if(request is VaulProcessingJobDescription vaultProcessingJobDescription)
                {
                    _logger.LogInformation($"Processing vault {vaultProcessingJobDescription.VaultId}...");

                    using var scope = _serviceProvider.CreateScope();
                    var job = scope.ServiceProvider.GetRequiredService<ProcessingJob>();

                    await job.RunAsync();
                }
                else
                {
                    _logger.LogWarning($"Unknown job description type {request.GetType().Name}.");
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An unhandled exception occurred");
            }
        }
    }
}
