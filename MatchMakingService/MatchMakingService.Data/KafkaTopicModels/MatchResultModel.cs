namespace MatchMakingService.Data.KafkaTopicModels;

[PublicAPI]
public record MatchResultModel
{
    public required string MatchID { get; init; }
    public required List<string> UserIDs { get; init; }
    public DateTime TimeStamp { get; set; } = DateTime.UtcNow;
}