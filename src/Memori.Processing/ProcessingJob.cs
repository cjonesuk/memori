using Memori.Data;
using Microsoft.Extensions.Logging;

namespace Memori.Processing;

public sealed class ProcessingJob
{
    private readonly ILogger _logger;
    private readonly VaulProcessingJobDescription _description;
    private readonly DatabaseContext _database;

    public ProcessingJob(ILogger<ProcessingJob> logger, VaulProcessingJobDescription description, DatabaseContext database)
    {
        _logger = logger;
        _description = description;
        _database = database;
    }

    public async Task RunAsync()
    {
        using (_logger.BeginScope(_description))
        {
            _logger.LogInformation($"Processing job is running...");

            var vault = await _database.FindVaultByIdAsync(_description.VaultId);

            if (vault == null)
            {
                _logger.LogError($"Vault with id {_description.VaultId} not found.");
                return;
            }

            _logger.LogDebug($"Vault {_description.VaultId} found.");

            var rootDirectory = new DirectoryInfo(vault.OriginalsPath);

            if (!rootDirectory.Exists)
            {
                _logger.LogError($"Vault {_description.VaultId} originals path {_description.VaultId} not found.");
                return;
            }

            _logger.LogInformation($"Processing job for Vault {_description.VaultId} is done.");
        }
    }

    private async Task<Asset?> FindAssetByPathAsync(string path)
    {
        return await _database.FindAssetByPathAsync(path);
    }

}