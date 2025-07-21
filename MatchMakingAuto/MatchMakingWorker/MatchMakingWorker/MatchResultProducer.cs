namespace MatchMakingWorker;

public class MatchResultProducer(ILogger<MatchResultProducer> logger, IConfiguration configuration, MatchUserConsumer consumer) : MatchMakingWorkerBase(logger)
{
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation(Constants.LogMessages.ServiceRunning);

        var producerConfig = new ProducerConfig
        {
            BootstrapServers = configuration.GetConnectionString(
                Constants.Configuration.ConnectionStrings.Kafka),
        };

        using var producer = new ProducerBuilder<string, string>(producerConfig).Build();

        while (!cancellationToken.IsCancellationRequested)
            await DoWorkAsync(producer, cancellationToken);
    }

    private async Task DoWorkAsync(IProducer<string, string> producer, CancellationToken cancellationToken)
    {
        logger.LogDebug(Constants.LogMessages.ServiceWorking);

        var matchMakingCompleteTopicName = configuration.GetValue<string>(
            Constants.Configuration.MatchMaking.KafkaTopics.MatchMakingComplete);

        var groupSize = configuration.GetValue<int>(
            Constants.Configuration.MatchMaking.GroupSize);

        while (consumer.ConsumedUsers.Count < groupSize)
        {
            var usersCheckDelay = TimeSpan.FromSeconds(1);
            await Task.Delay(usersCheckDelay, cancellationToken);
        }

        var matchID = Guid.NewGuid().ToString();
        var userIDs = new List<string>(groupSize);
        var timestamp = DateTime.UtcNow;

        lock (consumer.ConsumedUsersLock)
        {
            for (var i = 0; i < groupSize; i++)
            {
                var usersQueue = consumer.ConsumedUsers;
                if (!usersQueue.TryDequeue(out var userID)) continue;
                userIDs.Add(userID);
            }
        }

        try
        {
            await producer.ProduceAsync(
                matchMakingCompleteTopicName,
                new Message<string, string>
                {
                    Key = matchID,
                    Value = JsonConvert.SerializeObject(new
                    {
                        MatchID = matchID,
                        UserIDs = userIDs,
                        Timestamp = timestamp,
                    }),
                }, cancellationToken);

            using (logger.BeginScope("-"))
            {
                logger.LogInformation(Constants.LogMessages.MatchResultProduced, matchID);
                foreach (var matchUserID in userIDs)
                    logger.LogInformation(Constants.LogMessages.MatchResultProducedDetails,
                        userIDs.IndexOf(matchUserID) + 1, userIDs.Count, matchUserID);
            }
        }
        catch (ProduceException<string, string> ex)
        {
            using (logger.BeginScope("-"))
            {
                logger.LogError(Constants.LogMessages.MatchResultProductionError, matchID);
                logger.LogError(Constants.LogMessages.FailureReason, ex.Error.Reason);
            }
        }
    }
}