namespace MatchMakingService.Data.DataTransferObjects;

[PublicAPI]
public record ResponseMatchSearchDTO
{
    public bool Success => ErrorMessage is null;

    public string? ErrorMessage { get; init; }

    #region Helpers

    public static ResponseMatchSearchDTO OK() => new();

    public static ResponseMatchSearchDTO Error(string errorMessage) =>
        new() { ErrorMessage = errorMessage };

    #endregion
}