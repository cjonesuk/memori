using Memori.Data;
using Microsoft.Extensions.Logging;

namespace Memori.Processing;

public sealed class ProcessAllVaultsJob
{
    private readonly ILogger _logger;
    private readonly ProcessAllVaultsJobDescription _description;
    private readonly DatabaseContext _database;
    private readonly IProcessingManagerBackgroundService _processingManager;

    public ProcessAllVaultsJob(
        ILogger<ProcessAllVaultsJob> logger,
        ProcessAllVaultsJobDescription description,
        DatabaseContext database,
        IProcessingManagerBackgroundService processingManager)
    {
        _logger = logger;
        _description = description;
        _database = database;
        _processingManager = processingManager;
    }

    public async Task RunAsync()
    {
        _logger.LogInformation($"Processing job is running...");

        var vaults = await _database.FindAllVaultsAsync();

        foreach (var vault in vaults)
        {
            _logger.LogDebug($"Processing vault {vault.Id}.");

            var jobDescription = new VaultProcessingJobDescription(vault.Id);

            var success = _processingManager.RequestJob(jobDescription);

            if (!success)
            {
                _logger.LogError($"Failed to create processing job for vault {vault.Id}.");
                _logger.LogError($"Aborting processing job for all vaults.");
                return;
            }
        }

        _logger.LogInformation($"Processing job for all vaults is done.");

    }
}
