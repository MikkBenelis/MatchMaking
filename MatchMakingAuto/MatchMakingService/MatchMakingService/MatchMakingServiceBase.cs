namespace MatchMakingService;

public abstract class MatchMakingServiceBase(ILogger logger, IConfiguration configuration) : BackgroundService
{
    public override Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation(Constants.LogMessages.ServiceStarted);
        Task.Run(() => base.StartAsync(cancellationToken), cancellationToken);
        return Task.CompletedTask;
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogWarning(Constants.LogMessages.ServiceStopping);
        await base.StopAsync(cancellationToken);
        logger.LogInformation(Constants.LogMessages.ServiceStopped);
    }

    protected async Task EnsureTopicCreatedAsync(string topicName, int numPartitions, short replicationFactor)
    {
        var adminConfig = new AdminClientConfig
        {
            BootstrapServers = configuration.GetConnectionString(
                Constants.Configuration.ConnectionStrings.KafkaName),
        };

        using var adminClient = new AdminClientBuilder(adminConfig).Build();
        try
        {
            var metadata = adminClient.GetMetadata(TimeSpan.FromSeconds(10));
            if (metadata.Topics.Exists(topicMetadata => topicMetadata.Topic == topicName))
                return;

            await adminClient.CreateTopicsAsync([
                new TopicSpecification
                {
                    Name = topicName,
                    NumPartitions = numPartitions,
                    ReplicationFactor = replicationFactor,
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