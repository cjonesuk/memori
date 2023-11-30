using Microsoft.Extensions.Logging;

namespace Memori.Processing;

public static partial class ProcessingLogs
{
    [LoggerMessage(LogLevel.Information, "Add request for job [{jobRequestId}]")]
    public static partial void LogJobRequest(this ILogger logger, Guid jobRequestId);


    [LoggerMessage(LogLevel.Error, "Failed to add request for job [{wrapper}]")]
    public static partial void LogJobRequestError(this ILogger logger, JobRequest wrapper);
}
