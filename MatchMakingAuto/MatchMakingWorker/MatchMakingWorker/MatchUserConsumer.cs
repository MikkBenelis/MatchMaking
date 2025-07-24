namespace MatchMakingWorker;

#pragma warning disable CS9107

public class MatchUserConsumer(ILogger<MatchUserConsumer> logger, IConfiguration configuration)
    : MatchMakingWorkerBase(logger, configuration)
{
    public readonly ConcurrentQueue<string> ConsumedUsers = [];

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation(Constants.LogMessages.ServiceRunning);

        #region Ensure match making request topic created

        var matchMakingRequestTopicName = configuration.GetValue<string>(
            Constants.Configuration.MatchMaking.KafkaTopics.MatchMakingRequest.NameKey)!;

        var matchMakingRequestTopicNumPartitions = configuration.GetValue<int>(
            Constants.Configuration.MatchMaking.KafkaTopics.MatchMakingRequest.NumPartitionsKey);

        var matchMakingRequestTopicReplicationFactor = configuration.GetValue<short>(
            Constants.Configuration.MatchMaking.KafkaTopics.MatchMakingRequest.ReplicationFactorKey);

        await EnsureTopicCreatedAsync(
            matchMakingRequestTopicName,
            matchMakingRequestTopicNumPartitions,
            matchMakingRequestTopicReplicationFactor);

        #endregion

        var config = new ConsumerConfig
        {
            GroupId = nameof(MatchMakingWorker),
            BootstrapServers = configuration.GetConnectionString(
                Constants.Configuration.ConnectionStrings.KafkaName),
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
            var value = JsonConvert.DeserializeObject<dynamic>(result.Message.Value);
            var userID = (string)value!.UserID;

            lock (ConsumedUsers)
            {
                if (ConsumedUsers.Contains(userID))
                {
                    logger.LogWarning(Constants.LogMessages.MatchUserConsumedAlready, userID);
                    return Task.CompletedTask;
                }

                ConsumedUsers.Enqueue(userID);
            }

            logger.LogInformation(Constants.LogMessages.MatchUserConsumed, userID);
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