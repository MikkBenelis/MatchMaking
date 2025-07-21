namespace MatchMakingService;

public class MatchResultConsumer(ILogger<MatchResultConsumer> logger, IConfiguration configuration) : MatchMakingServiceBase(logger)
{
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation(Constants.LogMessages.ServiceRunning);

        var matchMakingCompleteTopicName = configuration.GetValue<string>(
            Constants.Configuration.MatchMaking.KafkaTopics.MatchMakingComplete)!;
        await EnsureTopicCreatedAsync(matchMakingCompleteTopicName);

        var consumerConfig = new ConsumerConfig
        {
            GroupId = nameof(MatchMakingService),
            BootstrapServers = configuration.GetConnectionString(
                Constants.Configuration.ConnectionStrings.Kafka),
            AutoOffsetReset = AutoOffsetReset.Earliest,
        };

        using var consumer = new ConsumerBuilder<string, string>(consumerConfig).Build();
        consumer.Subscribe(matchMakingCompleteTopicName);

        while (!cancellationToken.IsCancellationRequested)
            await DoWorkAsync(consumer, cancellationToken);
    }

    private Task DoWorkAsync(IConsumer<string, string> consumer, CancellationToken cancellationToken)
    {
        logger.LogDebug(Constants.LogMessages.ServiceWorking);

        try
        {
            var result = consumer.Consume(cancellationToken);
            var value = JsonConvert.DeserializeObject<JObject>(result.Message.Value);
            var matchID = value?.GetValue("MatchID")?.Value<string>() ?? "UNKNOWN";
            var userIDs = value?.GetValue("UserIDs")?
                .Values<string>().ToList() ?? ["UNKNOWN"];

            using (logger.BeginScope("-"))
            {
                logger.LogInformation(Constants.LogMessages.MatchResultConsumed, matchID);
                foreach (var matchUserID in userIDs)
                    logger.LogInformation(Constants.LogMessages.MatchResultConsumedDetails,
                        userIDs.IndexOf(matchUserID) + 1, userIDs.Count, matchUserID);
            }
        }
        catch (ConsumeException ex)
        {
            using (logger.BeginScope("-"))
            {
                logger.LogError(Constants.LogMessages.MatchResultConsumptionError);
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