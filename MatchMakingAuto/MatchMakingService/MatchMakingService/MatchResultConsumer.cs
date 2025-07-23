namespace MatchMakingService;

#pragma warning disable CS9107

public class MatchResultConsumer(ILogger<MatchResultConsumer> logger, IConfiguration configuration)
    : MatchMakingServiceBase(logger, configuration)
{
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation(Constants.LogMessages.ServiceRunning);

        #region Ensure match making complete topic created

        var matchMakingCompleteTopicName = configuration.GetValue<string>(
            Constants.Configuration.MatchMaking.KafkaTopics.MatchMakingComplete.NameKey)!;

        var matchMakingCompleteTopicNumPartitions = configuration.GetValue<int>(
            Constants.Configuration.MatchMaking.KafkaTopics.MatchMakingComplete.NumPartitionsKey);

        var matchMakingCompleteTopicReplicationFactor = configuration.GetValue<short>(
            Constants.Configuration.MatchMaking.KafkaTopics.MatchMakingComplete.ReplicationFactorKey);

        await EnsureTopicCreatedAsync(
            matchMakingCompleteTopicName,
            matchMakingCompleteTopicNumPartitions,
            matchMakingCompleteTopicReplicationFactor);

        #endregion

        var consumerConfig = new ConsumerConfig
        {
            GroupId = nameof(MatchMakingService),
            BootstrapServers = configuration.GetConnectionString(
                Constants.Configuration.ConnectionStrings.KafkaName),
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
}