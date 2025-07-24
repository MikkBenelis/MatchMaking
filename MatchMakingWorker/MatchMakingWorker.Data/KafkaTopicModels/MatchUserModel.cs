namespace MatchMakingWorker.Data.KafkaTopicModels;

[PublicAPI]
public record MatchUserModel
{
    public required string UserID { get; init; }

    public DateTime TimeStamp { get; init; } = DateTime.UtcNow;
}