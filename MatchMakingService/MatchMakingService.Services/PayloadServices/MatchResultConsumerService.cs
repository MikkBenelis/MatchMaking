namespace MatchMakingService.Services.PayloadServices;

#pragma warning disable CS9107

[PublicAPI]
public class MatchResultConsumerService(
    ILogger<MatchResultConsumerService> logger,
    IConfiguration configuration)
    : MatchMakingKafkaConsumerBase(logger, configuration)
{
    public readonly ConcurrentBag<MatchResultModel> MatchesList = [];

    public ResponseMatchStatusDTO GetMatchForUser(string userID)
    {
        var matchResult = MatchesList.FirstOrDefault(match =>
            match.UserIDs.Contains(userID));

        return matchResult is null
            ? ResponseMatchStatusDTO.Error(Constants.APIMessages.UserIsNotPlaying)
            : ResponseMatchStatusDTO.Match(matchResult.MatchID);
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        var matchMakingCompleteTopicName = configuration.GetValue<string>(
            Constants.Configuration.MatchMaking.KafkaTopics.MatchMakingCompleteKey)!;

        await base.ExecuteAsync(matchMakingCompleteTopicName, cancellationToken);
    }

    protected override async Task DoWorkAsync(CancellationToken cancellationToken)
    {
        await base.DoWorkAsync(cancellationToken);

        try
        {
            var result = Consumer.Consume(cancellationToken);
            var matchResultSerialized = result.Message.Value;
            var matchResult = JsonConvert.DeserializeObject
                <MatchResultModel>(matchResultSerialized)!;

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
    }
}