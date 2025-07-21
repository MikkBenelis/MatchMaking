namespace MatchMakingService.Data.DataTransferObjects;

[PublicAPI]
public record ResponseMatchStatusDTO
{
    public string? MatchID { get; init; }
    public required bool Success { get; init; }
    public string? ErrorMessage { get; init; }
}