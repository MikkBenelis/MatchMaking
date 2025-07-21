namespace MatchMakingWorker.Services;

[PublicAPI]
public class MatchResultProducerService(
    ILogger<MatchResultProducerService> logger,
    IConfiguration configuration,
    MatchUserConsumerService consumer)
    : MatchMakingServiceBase(logger)
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

        var matchResult = new MatchResultModel
        {
            MatchID = Guid.NewGuid().ToString(),
            UserIDs = new List<string>(groupSize),
        };

        lock (consumer.ConsumedUsersLock)
        {
            for (var i = 0; i < groupSize; i++)
            {
                var usersQueue = consumer.ConsumedUsers;
                if (!usersQueue.TryDequeue(out var userID)) continue;
                matchResult.UserIDs.Add(userID);
            }
        }

        var matchResultSerialized = JsonConvert.SerializeObject(matchResult);

        try
        {
            await producer.ProduceAsync(
                matchMakingCompleteTopicName,
                new Message<string, string>
                {
                    Key = matchResult.MatchID,
                    Value = matchResultSerialized,
                }, cancellationToken);

            using (logger.BeginScope("-"))
            {
                logger.LogInformation(Constants.LogMessages.MatchResultProduced, matchResult.MatchID);
                foreach (var matchUserID in matchResult.UserIDs)
                    logger.LogInformation(Constants.LogMessages.MatchResultProducedDetails,
                        matchResult.UserIDs.IndexOf(matchUserID) + 1,
                        matchResult.UserIDs.Count, matchUserID);
            }
        }
        catch (ProduceException<string, string> ex)
        {
            using (logger.BeginScope("-"))
            {
                logger.LogError(Constants.LogMessages.MatchResultProductionError, matchResult.MatchID);
                logger.LogError(Constants.LogMessages.FailureReason, ex.Error.Reason);
            }
        }
    }
}