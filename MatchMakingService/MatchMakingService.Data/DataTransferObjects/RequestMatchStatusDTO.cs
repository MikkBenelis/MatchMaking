namespace MatchMakingService.Data.DataTransferObjects;

[PublicAPI]
public record RequestMatchStatusDTO
{
    public required string UserID { get; init; }
}