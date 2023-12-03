using Memori.Processing.Indexing;
using Memori.Processing.Thumbnails;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading.Channels;

namespace Memori.Processing;

public sealed class JobManager : BackgroundService, IJobManager
{
    private readonly ILogger _logger;
    private readonly ILogger _requestLogger;
    private readonly IServiceProvider _serviceProvider;
    private readonly Channel<JobRequest> _requests;

    public JobManager(ILoggerFactory loggerFactory, IServiceProvider serviceProvider)
    {
        _logger = loggerFactory.CreateLogger<JobManager>();
        _requestLogger = loggerFactory.CreateLogger<JobManager>();
        _serviceProvider = serviceProvider;
        _requests = Channel.CreateUnbounded<JobRequest>();
    }

    public bool RequestJob(IJobDescription jobDescription)
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
                await RunJob(request.Description);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An unhandled exception occurred");
            }
        }
    }

    private Task RunJob(IJobDescription description)
    {
        return description switch
        {
            IndexVaultJobDescription job => RunIndexVaultJob(job),
            IndexAllVaultsJobDescription job => RunIndexAllVaultsJob(job),
            GenerateThumbnailsJobDescription job => RunThumbnailGenerationJob(job),
            _ => throw new ArgumentException($"Unknown job description type {description.GetType().Name}.", nameof(description)),
        };
    }

    private async Task RunIndexAllVaultsJob(IndexAllVaultsJobDescription description)
    {
        using var scope = _serviceProvider.CreateScope();

        var job = ActivatorUtilities.CreateInstance<IndexAllVaultsJob>(scope.ServiceProvider, description);

        await job.RunAsync();
    }

    private async Task RunIndexVaultJob(IndexVaultJobDescription description)
    {
        using var scope = _serviceProvider.CreateScope();

        var job = ActivatorUtilities.CreateInstance<IndexVaultJob>(scope.ServiceProvider, description);

        await job.RunAsync();
    }

    private async Task RunThumbnailGenerationJob(GenerateThumbnailsJobDescription description)
    {
        using var scope = _serviceProvider.CreateScope();

        var job = ActivatorUtilities.CreateInstance<GenerateThumbnailsJob>(scope.ServiceProvider, description);

        await job.RunAsync();
    }
}
