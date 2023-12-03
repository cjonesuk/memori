using Memori.Data;
using Microsoft.Extensions.Logging;

namespace Memori.Processing.Indexing;

public sealed class IndexAllVaultsJob : PerVaultJobBase
{
    private readonly IJobManager jobManager;

    public IndexAllVaultsJob(
        ILogger<IndexAllVaultsJob> logger,
        DatabaseContext database,
        IJobManager jobManager,
        IndexAllVaultsJobDescription description) : base(logger, database)
    {
        this.jobManager = jobManager;
    }

    protected override Task<Result> ProcessVaultAsync(Vault vault)
    {
        var jobDescription = new IndexVaultJobDescription(vault.Id);

        var success = jobManager.RequestJob(jobDescription);

        if (!success)
        {
            _logger.LogError($"Failed to create processing job for vault {vault.Id}.");
            _logger.LogError($"Aborting processing job for all vaults.");
            return Task.FromResult(Result.Stop);
        }

        return Task.FromResult(Result.Continue);
    }
}
