namespace MatchMakingWorker.Data.KafkaTopicModels;

[PublicAPI]
public record MatchResultModel
{
    public required string MatchID { get; init; }

    public required List<string> UserIDs { get; init; }

    public DateTime TimeStamp { get; init; } = DateTime.UtcNow;
}