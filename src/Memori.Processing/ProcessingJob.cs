namespace Memori.Processing;

public sealed class ProcessingJob
{
    public async Task RunAsync()
    {
        Console.WriteLine("Processing job is running...");
        await Task.Delay(5000);
        Console.WriteLine("Processing job is done.");
    }

}