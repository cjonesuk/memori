namespace Memori.Processing;

public interface IProcessingManagerBackgroundService
{
    bool RequestJob(IProcessingJobDescription jobDescription);
}
