namespace MatchMakingService.Data.DataTransferObjects;

[PublicAPI]
public record RequestMatchSearchDTO
{
    public required string UserID { get; init; }
}