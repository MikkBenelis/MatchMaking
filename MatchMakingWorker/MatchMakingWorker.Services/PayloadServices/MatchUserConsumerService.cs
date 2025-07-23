namespace MatchMakingWorker.Services.PayloadServices;

#pragma warning disable CS9107

public class MatchUserConsumerService(
    ILogger<MatchUserConsumerService> logger,
    IConfiguration configuration)
    : MatchMakingKafkaConsumerBase(logger, configuration)
{
    public readonly ConcurrentQueue<string> ConsumedUsers = [];

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        var matchMakingRequestTopicName = configuration.GetValue<string>(
            Constants.Configuration.MatchMaking.KafkaTopics.MatchMakingRequestKey)!;

        await base.ExecuteAsync(matchMakingRequestTopicName, cancellationToken);
    }

    protected override async Task DoWorkAsync(CancellationToken cancellationToken)
    {
        await base.DoWorkAsync(cancellationToken);

        try
        {
            var result = Consumer.Consume(cancellationToken);
            var matchUserSerialized = result.Message.Value;
            var matchUser = JsonConvert.DeserializeObject
                <MatchUserModel>(matchUserSerialized)!;

            lock (ConsumedUsers)
            {
                if (ConsumedUsers.Contains(matchUser.UserID))
                {
                    logger.LogWarning(Constants.LogMessages.MatchUserConsumedAlready, matchUser.UserID);
                    return;
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
    }
}