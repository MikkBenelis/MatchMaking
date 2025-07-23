namespace MatchMakingService.Services.InfrastructureServices;

#pragma warning disable CS9107

public abstract class MatchMakingKafkaConsumerBase(
    ILogger<MatchMakingKafkaConsumerBase> logger,
    IConfiguration configuration)
    : MatchMakingServiceBase(logger)
{
    protected IConsumer<string, string> Consumer = null!;

    protected async Task ExecuteAsync(string topicName, CancellationToken cancellationToken)
    {
        logger.LogInformation(Constants.LogMessages.ServiceRunning);

        var consumerConfig = new ConsumerConfig
        {
            GroupId = nameof(MatchMakingService),
            BootstrapServers = configuration.GetConnectionString(
                Constants.Configuration.ConnectionStrings.KafkaName),
            AutoOffsetReset = AutoOffsetReset.Earliest,
        };

        using (Consumer = new ConsumerBuilder<string, string>(consumerConfig).Build())
        {
            Consumer.Subscribe(topicName);

            while (!cancellationToken.IsCancellationRequested)
                await DoWorkAsync(cancellationToken);
        }
    }

    protected virtual Task DoWorkAsync(CancellationToken cancellationToken)
    {
        logger.LogDebug(Constants.LogMessages.ServiceWorking);

        return Task.CompletedTask;
    }
}