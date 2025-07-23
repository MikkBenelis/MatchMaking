namespace MatchMakingWorker.Services.InfrastructureServices;

#pragma warning disable CS9107

public abstract class MatchMakingKafkaProducerBase(
    ILogger<MatchMakingKafkaProducerBase> logger,
    IConfiguration configuration)
    : MatchMakingServiceBase(logger)
{
    protected IProducer<string, string> Producer = null!;

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation(Constants.LogMessages.ServiceRunning);

        var producerConfig = new ProducerConfig
        {
            BootstrapServers = configuration.GetConnectionString(
                Constants.Configuration.ConnectionStrings.KafkaName),
        };

        using (Producer = new ProducerBuilder<string, string>(producerConfig).Build())
            while (!cancellationToken.IsCancellationRequested)
                await DoWorkAsync(cancellationToken);
    }

    protected virtual Task DoWorkAsync(CancellationToken cancellationToken)
    {
        logger.LogDebug(Constants.LogMessages.ServiceWorking);

        return Task.CompletedTask;
    }
}