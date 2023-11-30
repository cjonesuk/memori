using Microsoft.Extensions.Logging;

namespace Memori.Processing;

public static class LogExtensions
{
    public static IDisposable? BeginScoped(this ILogger logger, params (string Key, object Value)[] values)
    {
        return logger.BeginScope(values.ToDictionary(x => x.Key, x => x.Value));
    }
}