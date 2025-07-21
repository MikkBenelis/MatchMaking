namespace MatchMakingService.Services;

[PublicAPI]
public class MatchUserProducerService(
    ILogger<MatchUserProducerService> logger,
    IConfiguration configuration,
    MatchResultConsumerService consumer)
    : MatchMakingServiceBase(logger)
{
    public readonly ConcurrentQueue<MatchUserModel> UsersQueue = [];
    public readonly ConcurrentQueue<MatchUserModel> UsersQueueHistory = [];

    public readonly Lock UsersQueueLock = new();
    public readonly Lock UsersQueueHistoryLock = new();

    public ResponseMatchSearchDTO AddUserToQueue(MatchUserModel user)
    {
        lock (consumer.MatchesListLock)
        {
            if (consumer.MatchesList.Any(m => m.UserIDs.Contains(user.UserID)))
            {
                return new ResponseMatchSearchDTO
                {
                    Success = false,
                    ErrorMessage = Constants.APIMessages.UserIsAlreadyPlaying,
                };
            }
        }

        lock (UsersQueueHistoryLock)
        {
            if (UsersQueueHistory.Any(u => u.UserID == user.UserID))
            {
                return new ResponseMatchSearchDTO
                {
                    Success = false,
                    ErrorMessage = Constants.APIMessages.UserIsAlreadySearching,
                };
            }
        }

        lock (UsersQueueLock)
            UsersQueue.Enqueue(user);

        lock (UsersQueueHistoryLock)
            UsersQueueHistory.Enqueue(user);

        return new ResponseMatchSearchDTO { Success = true };
    }

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

        var matchMakingRequestTopicName = configuration.GetValue<string>(
            Constants.Configuration.MatchMaking.KafkaTopics.MatchMakingRequest);

        while (!cancellationToken.IsCancellationRequested)
        {
            lock (UsersQueueLock)
                if (!UsersQueue.IsEmpty)
                    break;

            var usersCheckDelay = TimeSpan.FromSeconds(1);
            await Task.Delay(usersCheckDelay, cancellationToken);
        }

        MatchUserModel? matchUser;
        lock (UsersQueueLock)
            if (!UsersQueue.TryDequeue(out matchUser))
                return;

        try
        {
            await producer.ProduceAsync(
                matchMakingRequestTopicName,
                new Message<string, string>
                {
                    Key = matchUser.UserID,
                    Value = JsonConvert.SerializeObject(matchUser),
                }, cancellationToken);

            logger.LogInformation(Constants.LogMessages.MatchUserProduced, matchUser.UserID);
        }
        catch (ProduceException<string, string> ex)
        {
            using (logger.BeginScope("-"))
            {
                logger.LogError(Constants.LogMessages.MatchUserProductionError, matchUser.UserID);
                logger.LogError(Constants.LogMessages.FailureReason, ex.Error.Reason);
            }
        }
    }
}