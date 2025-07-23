namespace MatchMakingService.Services.InfrastructureServices;

public abstract class MatchMakingServiceBase(ILogger logger) : BackgroundService
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
}