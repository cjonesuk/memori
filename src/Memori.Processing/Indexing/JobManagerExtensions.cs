namespace Memori.Processing.Indexing;

public static class JobManagerExtensions
{
    public static bool RequestIndexAllVaults(this IJobManager processingManager)
    {
        return processingManager.RequestJob(new IndexAllVaultsJobDescription());
    }
}