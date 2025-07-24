namespace MatchMakingWorker.Services.InfrastructureServices;

public class MatchMakingRedisLogger(string categoryName, IServiceProvider serviceProvider) : ILogger
{
    private readonly IConfiguration _configuration = serviceProvider
        .GetRequiredService<IConfiguration>();

    private readonly Lazy<IDatabase> _redisDB = new(() => serviceProvider
        .GetRequiredService<IConnectionMultiplexer>()
        .GetDatabase());

    public bool IsEnabled(LogLevel logLevel)
    {
        const string isActiveConfigKey = Constants.Configuration.MatchMaking.RedisLogging.IsActiveKey;
        return _configuration.GetValue<bool>(isActiveConfigKey) && categoryName.Contains(nameof(MatchMakingWorker));
    }

    public IDisposable BeginScope<TState>(TState state) where TState : notnull => EmptyScope.Instance;

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
        var logDuration = TimeSpan.FromHours(_configuration.GetValue<int>(
            Constants.Configuration.MatchMaking.RedisLogging.LifetimeHoursKey));

        _redisDB.Value.StringSetAsync(logKey, logValue, logDuration);
    }

    public sealed class Provider(IServiceProvider serviceProvider) : ILoggerProvider
    {
        private readonly ConcurrentDictionary<string, ILogger> _loggers = new();

        private bool _isDisposed;

        public ILogger CreateLogger(string categoryName)
        {
            return _loggers.GetOrAdd(categoryName, _ =>
                new MatchMakingRedisLogger(categoryName, serviceProvider));
        }

        public void Dispose()
        {
            if (_isDisposed) return;
            _isDisposed = true;
            _loggers.Clear();
        }
    }

    private sealed class EmptyScope : IDisposable
    {
        public static EmptyScope Instance { get; } = new();

        private EmptyScope()
        {
            // Placeholder
        }

        public void Dispose()
        {
            // Placeholder
        }
    }
}

[PublicAPI]
public static class LoggerExtensions
{
    public static ILoggingBuilder AddMatchMakingRedisLoggerProvider(this ILoggingBuilder builder)
    {
        builder.Services.AddSingleton<ILoggerProvider, MatchMakingRedisLogger.Provider>();
        return builder;
    }
}