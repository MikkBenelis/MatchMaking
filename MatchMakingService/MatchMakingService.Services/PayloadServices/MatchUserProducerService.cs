namespace MatchMakingService.Services.PayloadServices;

#pragma warning disable CS9107

[PublicAPI]
public class MatchUserProducerService(
    ILogger<MatchUserProducerService> logger,
    IConfiguration configuration,
    MatchResultConsumerService consumer)
    : MatchMakingKafkaProducerBase(logger, configuration)
{
    public readonly ConcurrentQueue<MatchUserModel> UsersQueue = [];
    public readonly ConcurrentQueue<MatchUserModel> UsersQueueHistory = [];

    public ResponseMatchSearchDTO AddUserToQueue(MatchUserModel userModel)
    {
        if (consumer.MatchesList.Any(match => match.UserIDs.Contains(userModel.UserID)))
            return ResponseMatchSearchDTO.Error(Constants.APIMessages.UserIsAlreadyPlaying);

        if (UsersQueueHistory.Any(user => user.UserID == userModel.UserID))
            return ResponseMatchSearchDTO.Error(Constants.APIMessages.UserIsAlreadySearching);

        UsersQueue.Enqueue(userModel);
        UsersQueueHistory.Enqueue(userModel);

        return ResponseMatchSearchDTO.OK();
    }

    protected override async Task DoWorkAsync(CancellationToken cancellationToken)
    {
        await base.DoWorkAsync(cancellationToken);

        while (!cancellationToken.IsCancellationRequested)
        {
            if (!UsersQueue.IsEmpty)
                break;

            var usersCheckDelayTime = TimeSpan.FromSeconds(1);
            await Task.Delay(usersCheckDelayTime, cancellationToken);
        }

        if (!UsersQueue.TryDequeue(out var matchUser))
            return;

        try
        {
            var matchMakingRequestTopicName = configuration.GetValue<string>(
                Constants.Configuration.MatchMaking.KafkaTopics.MatchMakingRequestKey);

            await Producer.ProduceAsync(
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