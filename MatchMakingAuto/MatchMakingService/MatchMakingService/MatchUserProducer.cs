namespace MatchMakingService;

public class MatchUserProducer(ILogger<MatchUserProducer> logger, IConfiguration configuration) : MatchMakingServiceBase(logger)
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

        var minRequestDelay = configuration.GetValue<int>(Constants.Configuration.MatchMaking.RequestDelay.MinMS);
        var maxRequestDelay = configuration.GetValue<int>(Constants.Configuration.MatchMaking.RequestDelay.MaxMS);

        while (!cancellationToken.IsCancellationRequested)
        {
            var randomRequestDelay = Random.Shared.Next(minRequestDelay, maxRequestDelay);
            var delayTimeSpan = TimeSpan.FromMilliseconds(randomRequestDelay);
            await Task.Delay(delayTimeSpan, cancellationToken);

            await DoWorkAsync(producer, cancellationToken);
        }
    }

    private async Task DoWorkAsync(IProducer<string, string> producer, CancellationToken cancellationToken)
    {
        logger.LogDebug(Constants.LogMessages.ServiceWorking);

        var matchMakingRequestTopicName = configuration.GetValue<string>(
            Constants.Configuration.MatchMaking.KafkaTopics.MatchMakingRequest);

        var userID = Guid.NewGuid().ToString();
        var timestamp = DateTime.UtcNow;

        try
        {
            await producer.ProduceAsync(
                matchMakingRequestTopicName,
                new Message<string, string>
                {
                    Key = userID,
                    Value = JsonConvert.SerializeObject(new
                    {
                        UserID = userID,
                        Timestamp = timestamp,
                    }),
                }, cancellationToken);

            logger.LogInformation(Constants.LogMessages.MatchUserProduced, userID);
        }
        catch (ProduceException<string, string> ex)
        {
            using (logger.BeginScope("-"))
            {
                logger.LogError(Constants.LogMessages.MatchUserProductionError, userID);
                logger.LogError(Constants.LogMessages.FailureReason, ex.Error.Reason);
            }
        }
    }
}