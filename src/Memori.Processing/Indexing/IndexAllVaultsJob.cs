using Memori.Data;
using Microsoft.Extensions.Logging;

namespace Memori.Processing.Indexing;

public sealed class IndexAllVaultsJob
{
    private readonly ILogger _logger;
    private readonly IndexAllVaultsJobDescription _description;
    private readonly DatabaseContext _database;
    private readonly IJobManager _processingManager;

    public IndexAllVaultsJob(
        ILogger<IndexAllVaultsJob> logger,
        IndexAllVaultsJobDescription description,
        DatabaseContext database,
        IJobManager processingManager)
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

            var jobDescription = new IndexVaultJobDescription(vault.Id);

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
