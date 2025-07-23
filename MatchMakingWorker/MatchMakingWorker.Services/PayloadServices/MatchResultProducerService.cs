namespace MatchMakingWorker.Services.PayloadServices;

#pragma warning disable CS9107

public class MatchResultProducerService(
    ILogger<MatchResultProducerService> logger,
    IConfiguration configuration,
    MatchUserConsumerService consumer)
    : MatchMakingKafkaProducerBase(logger, configuration)
{
    protected override async Task DoWorkAsync(CancellationToken cancellationToken)
    {
        await base.DoWorkAsync(cancellationToken);

        var groupSize = configuration.GetValue<int>(
            Constants.Configuration.MatchMaking.GroupSizeKey);

        while (consumer.ConsumedUsers.Count < groupSize)
        {
            var usersCheckDelayTime = TimeSpan.FromSeconds(1);
            await Task.Delay(usersCheckDelayTime, cancellationToken);
        }

        var matchResult = new MatchResultModel
        {
            MatchID = Guid.NewGuid().ToString(),
            UserIDs = new List<string>(groupSize),
        };

        lock (consumer.ConsumedUsers)
        {
            for (var i = 0; i < groupSize; i++)
            {
                var usersQueue = consumer.ConsumedUsers;
                if (!usersQueue.TryDequeue(out var userID)) continue;
                matchResult.UserIDs.Add(userID);
            }
        }

        try
        {
            var matchMakingCompleteTopicName = configuration.GetValue<string>(
                Constants.Configuration.MatchMaking.KafkaTopics.MatchMakingCompleteKey);
            var matchResultSerialized = JsonConvert.SerializeObject(matchResult);

            await Producer.ProduceAsync(
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