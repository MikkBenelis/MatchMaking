namespace MatchMakingService.Data.DataTransferObjects;

[PublicAPI]
public record ResponseMatchStatusDTO
{
    public bool Success => ErrorMessage is null;

    public string? MatchID { get; init; }

    public string? ErrorMessage { get; init; }

    #region Helpers

    public static ResponseMatchStatusDTO Match(string matchID) =>
        new() { MatchID = matchID };

    public static ResponseMatchStatusDTO Error(string errorMessage) =>
        new() { ErrorMessage = errorMessage };

    #endregion
}