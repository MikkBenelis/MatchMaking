namespace MatchMakingWorker.Services;

[PublicAPI]
public class MatchUserConsumerService(
    ILogger<MatchUserConsumerService> logger,
    IConfiguration configuration)
    : MatchMakingServiceBase(logger)
{
    public readonly ConcurrentQueue<string> ConsumedUsers = [];
    public readonly Lock ConsumedUsersLock = new();

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation(Constants.LogMessages.ServiceRunning);

        var matchMakingRequestTopicName = configuration.GetValue<string>(
            Constants.Configuration.MatchMaking.KafkaTopics.MatchMakingRequest)!;

        var config = new ConsumerConfig
        {
            GroupId = nameof(MatchMakingWorker),
            BootstrapServers = configuration.GetConnectionString(
                Constants.Configuration.ConnectionStrings.Kafka),
            AutoOffsetReset = AutoOffsetReset.Earliest,
        };

        using var consumer = new ConsumerBuilder<string, string>(config).Build();
        consumer.Subscribe(matchMakingRequestTopicName);

        while (!cancellationToken.IsCancellationRequested)
            await DoWorkAsync(consumer, cancellationToken);
    }

    private Task DoWorkAsync(IConsumer<string, string> consumer, CancellationToken cancellationToken)
    {
        logger.LogDebug(Constants.LogMessages.ServiceWorking);

        try
        {
            var result = consumer.Consume(cancellationToken);
            var matchUserSerialized = result.Message.Value;
            var matchUser = JsonConvert.DeserializeObject
                <MatchUserModel>(matchUserSerialized)!;

            lock (ConsumedUsersLock)
            {
                if (ConsumedUsers.Contains(matchUser.UserID))
                {
                    logger.LogWarning(Constants.LogMessages.MatchUserConsumedAlready, matchUser.UserID);
                    return Task.CompletedTask;
                }

                ConsumedUsers.Enqueue(matchUser.UserID);
            }

            logger.LogInformation(Constants.LogMessages.MatchUserConsumed, matchUser.UserID);
        }
        catch (ConsumeException ex)
        {
            using (logger.BeginScope("-"))
            {
                logger.LogError(Constants.LogMessages.MatchUserConsumptionError);
                logger.LogError(Constants.LogMessages.FailureReason, ex.Error.Reason);
            }
        }

        return Task.CompletedTask;
    }
}