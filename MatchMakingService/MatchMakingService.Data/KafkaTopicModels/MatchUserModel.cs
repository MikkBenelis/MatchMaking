namespace MatchMakingService.Data.KafkaTopicModels;

[PublicAPI]
public record MatchUserModel
{
    public required string UserID { get; init; }
    public DateTime TimeStamp { get; set; } = DateTime.UtcNow;
}