namespace MatchMakingWorker;

public class MatchUserConsumer(ILogger<MatchUserConsumer> logger, IConfiguration configuration) : MatchMakingWorkerBase(logger)
{
    public ConcurrentQueue<string> ConsumedUsers { get; } = [];
    public readonly Lock ConsumedUsersLock = new();

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation(Constants.LogMessages.ServiceRunning);

        var matchMakingRequestTopicName = configuration.GetValue<string>(
            Constants.Configuration.MatchMaking.KafkaTopics.MatchMakingRequest)!;
        await EnsureTopicCreatedAsync(matchMakingRequestTopicName);

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
            var value = JsonConvert.DeserializeObject<dynamic>(result.Message.Value);
            var userID = (string)value!.UserID;

            lock (ConsumedUsersLock)
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

    private async Task EnsureTopicCreatedAsync(string topicName)
    {
        var adminConfig = new AdminClientConfig
        {
            BootstrapServers = configuration.GetConnectionString(
                Constants.Configuration.ConnectionStrings.Kafka),
        };

        using var adminClient = new AdminClientBuilder(adminConfig).Build();
        try
        {
            var metadata = adminClient.GetMetadata(TimeSpan.FromSeconds(10));
            if (metadata.Topics.Exists(t => t.Topic == topicName))
                return;

            await adminClient.CreateTopicsAsync([
                new TopicSpecification
                {
                    Name = topicName,
                    ReplicationFactor = 1,
                    NumPartitions = 1,
                },
            ]);
        }
        catch (CreateTopicsException ex)
        {
            using (logger.BeginScope("-"))
            {
                logger.LogError(Constants.LogMessages.TopicCreationError);
                logger.LogError(Constants.LogMessages.FailureReason, ex.Error.Reason);
            }
        }
    }
}