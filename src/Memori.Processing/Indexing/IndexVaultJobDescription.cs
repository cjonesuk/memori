namespace Memori.Processing.Indexing;

public record IndexVaultJobDescription(string VaultId) : IJobDescription;

public record IndexAllVaultsJobDescription() : IJobDescription;


public static class ProcessingManagerBackgroundServiceExtensions
{
    public static bool RequestIndexAllVaults(this IJobManager processingManager)
    {
        return processingManager.RequestJob(new IndexAllVaultsJobDescription());
    }
}