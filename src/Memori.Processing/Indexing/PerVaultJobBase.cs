using Memori.Data;
using Microsoft.Extensions.Logging;

namespace Memori.Processing.Indexing;

public abstract class PerVaultJobBase
{
    protected readonly ILogger _logger;
    protected readonly DatabaseContext _database;

    protected PerVaultJobBase(
        ILogger logger,
        DatabaseContext database)
    {
        _logger = logger;
        _database = database;
    }

    protected enum Result
    {
        Continue,
        Stop
    }

    public async Task RunAsync()
    {
        _logger.LogInformation($"Processing job is running...");

        var vaults = await _database.FindAllVaultsAsync();

        foreach (var vault in vaults)
        {
            using (_logger.BeginScoped(("VaultId", vault.Id)))
            {
                _logger.LogDebug($"Processing vault {vault.Id}.");

                await ProcessVaultAsync(vault);
            }
        }

        _logger.LogInformation($"Processing job for all vaults is done.");
    }

    protected abstract Task<Result> ProcessVaultAsync(Vault vault);
}
