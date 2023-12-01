namespace Memori.Processing;

public record VaultIndexingJobDescription(string VaultId) : IProcessingJobDescription;

public record ProcessAllVaultsJobDescription() : IProcessingJobDescription;


public static class ProcessingManagerBackgroundServiceExtensions
{
    public static bool RequestIndexAllVaults(this IProcessingManagerBackgroundService processingManager)
    {
        return processingManager.RequestJob(new ProcessAllVaultsJobDescription());
    }
}