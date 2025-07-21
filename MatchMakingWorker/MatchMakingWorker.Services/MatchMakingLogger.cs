namespace MatchMakingWorker.Services;

public class MatchMakingLogger(string categoryName, IServiceProvider serviceProvider) : ILogger
{
    private readonly IDatabase _redisDB = serviceProvider
        .GetRequiredService<IConnectionMultiplexer>().GetDatabase();

    public bool IsEnabled(LogLevel logLevel) => categoryName.Contains("MatchMaking");

    public IDisposable BeginScope<TState>(TState state) where TState : notnull
    {
        return NullScope.Instance;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state,
        Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel))
            return;

        var logTimestamp = DateTime.UtcNow;
        var logTimestampDate = logTimestamp.ToString("yyyy.MM.dd");
        var logTimestampTime = logTimestamp.ToString("HH.mm.ss");

        var logKey = $"LOG:{categoryName}:{logLevel}:{logTimestampDate}:{Guid.NewGuid()}";
        var logValue = $"[{logTimestampDate}:{logTimestampTime}] {formatter(state, exception)}";
        var logDuration = TimeSpan.FromDays(1);

        _ = _redisDB.StringSetAsync(logKey, logValue, logDuration);
    }
}

public sealed class NullScope : IDisposable
{
    public static NullScope Instance { get; } = new();

    private NullScope()
    {
        // Placeholder
    }

    public void Dispose()
    {
        // Placeholder
    }
}

public sealed class MatchMakingLoggerProvider(IServiceProvider serviceProvider) : ILoggerProvider
{
    private readonly ConcurrentDictionary<string, ILogger> _loggers = new();

    private bool _isDisposed;

    public ILogger CreateLogger(string categoryName)
    {
        return _loggers.GetOrAdd(categoryName, _ =>
            new MatchMakingLogger(categoryName, serviceProvider));
    }

    public void Dispose()
    {
        if (_isDisposed) return;
        _isDisposed = true;
        _loggers.Clear();
    }
}

[PublicAPI]
public static class LoggerExtensions
{
    public static ILoggingBuilder AddMatchMakingLoggerProvider(this ILoggingBuilder builder)
    {
        builder.Services.AddSingleton<ILoggerProvider, MatchMakingLoggerProvider>();
        return builder;
    }
}