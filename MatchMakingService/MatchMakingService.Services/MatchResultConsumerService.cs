namespace MatchMakingService.Services;

[PublicAPI]
public class MatchResultConsumerService(
    ILogger<MatchResultConsumerService> logger,
    IConfiguration configuration)
    : MatchMakingServiceBase(logger)
{
    public readonly ConcurrentBag<MatchResultModel> MatchesList = [];
    public readonly Lock MatchesListLock = new();

    public ResponseMatchStatusDTO GetMatchForUser(string userID)
    {
        MatchResultModel? matchResult;
        lock (MatchesListLock)
            matchResult = MatchesList
                .FirstOrDefault(m =>
                    m.UserIDs.Contains(userID));

        if (matchResult is null)
        {
            return new ResponseMatchStatusDTO
            {
                Success = false,
                ErrorMessage = Constants.APIMessages.UserIsAlreadySearching,
            };
        }

        return new ResponseMatchStatusDTO
        {
            MatchID = matchResult.MatchID,
            Success = true,
        };
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation(Constants.LogMessages.ServiceRunning);

        var matchMakingCompleteTopicName = configuration.GetValue<string>(
            Constants.Configuration.MatchMaking.KafkaTopics.MatchMakingComplete)!;

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
            var matchResultSerialized = result.Message.Value;
            var matchResult = JsonConvert.DeserializeObject
                <MatchResultModel>(matchResultSerialized)!;

            lock (MatchesListLock)
                MatchesList.Add(matchResult);

            using (logger.BeginScope("-"))
            {
                logger.LogInformation(Constants.LogMessages.MatchResultConsumed, matchResult.MatchID);
                foreach (var matchUserID in matchResult.UserIDs)
                    logger.LogInformation(Constants.LogMessages.MatchResultConsumedDetails,
                        matchResult.UserIDs.IndexOf(matchUserID) + 1,
                        matchResult.UserIDs.Count, matchUserID);
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