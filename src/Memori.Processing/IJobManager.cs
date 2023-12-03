namespace Memori.Processing;

public interface IJobManager
{
    bool RequestJob(IJobDescription jobDescription);
}
