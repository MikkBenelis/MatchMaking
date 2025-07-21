namespace MatchMakingService.Data.DataTransferObjects;

[PublicAPI]
public record ResponseMatchSearchDTO
{
    public required bool Success { get; init; }
    public string? ErrorMessage { get; init; }
}