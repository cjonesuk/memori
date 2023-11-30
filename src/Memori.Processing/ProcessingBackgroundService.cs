using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading.Channels;

namespace Memori.Processing;

public sealed class ProcessingManagerBackgroundService : BackgroundService, IProcessingManagerBackgroundService
{
    private readonly ILogger _logger;
    private readonly ILogger _requestLogger;
    private readonly IServiceProvider _serviceProvider;
    private readonly Channel<JobRequest> _requests;

    public ProcessingManagerBackgroundService(ILoggerFactory loggerFactory, IServiceProvider serviceProvider)
    {
        _logger = loggerFactory.CreateLogger<ProcessingManagerBackgroundService>();
        _requestLogger = loggerFactory.CreateLogger<ProcessingManagerBackgroundService>();
        _serviceProvider = serviceProvider;
        _requests = Channel.CreateUnbounded<JobRequest>();
    }

    public bool RequestJob(IProcessingJobDescription jobDescription)
    {
        var request = new JobRequest(Guid.NewGuid(), jobDescription);

        _requestLogger.LogJobRequest(request.JobRequestId);

        var success = _requests.Writer.TryWrite(request);

        if (!success)
        {
            _requestLogger.LogJobRequestError(request);
        }

        return success;
    }


    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Processing background manager service is now running.");

        await foreach (var request in _requests.Reader.ReadAllAsync(stoppingToken))
        {
            _logger.BeginScoped(("JobRequestId", request.JobRequestId), ("JobRequestDescriptionType", request.Description.GetType().Name));

            try
            {
                switch (request.Description)
                {
                    case VaultIndexingJobDescription job:
                        await RunVaultProcessingJob(job);
                        break;

                    case ProcessAllVaultsJobDescription job:
                        await ProcessAllVaults(job);
                        break;

                    default:
                        _logger.LogWarning($"Unknown job description type {request.GetType().Name}.");
                        break;
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An unhandled exception occurred");
            }
        }
    }

    private async Task ProcessAllVaults(ProcessAllVaultsJobDescription description)
    {
        using var scope = _serviceProvider.CreateScope();

        var job = ActivatorUtilities.CreateInstance<ProcessAllVaultsJob>(scope.ServiceProvider, description);

        await job.RunAsync();
    }

    private async Task RunVaultProcessingJob(VaultIndexingJobDescription description)
    {
        using var scope = _serviceProvider.CreateScope();

        var job = ActivatorUtilities.CreateInstance<VaultIndexingJob>(scope.ServiceProvider, description);

        await job.RunAsync();
    }
}
