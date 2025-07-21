namespace MatchMakingService.Data;

public static class Constants
{
    public static class LogMessages
    {
        public const string ServiceStarted = "Service started!";
        public const string ServiceRunning = "Service is running!";
        public const string ServiceWorking = "Service is working...";
        public const string ServiceStopping = "Service stopping...";
        public const string ServiceStopped = "Service stopped!";

        public const string FailureReason = "Failure reason: {ErrorReason}";

        public const string MatchUserProduced = "Produced match request for UserID={UserID}!";
        public const string MatchUserProductionError = "Match request production failed for UserID={UserID}!";

        public const string MatchResultConsumed = "Consumed match result for MatchID={MatchID}!";
        public const string MatchResultConsumedDetails = "[{i}/{c}] Consumed match result UserID={UserID}";
        public const string MatchResultConsumptionError = "Match result consumption failed!";
    }

    public static class APIMessages
    {
        public const string UserIsAlreadySearching = "User is already searching!";
        public const string UserIsAlreadyPlaying = "User is already playing!";

        public const string UserIDIsRequired = "UserID is required!";
    }

    public static class Configuration
    {
        public static class ConnectionStrings
        {
            public const string Redis = "Redis";
            public const string Kafka = "Kafka";
        }

        public const string AllowSwagger = "AllowSwagger";
        public const string ForceHTTPSRedirect = "ForceHTTPSRedirect";

        public static class MatchMaking
        {
            public static class KafkaTopics
            {
                public const string MatchMakingRequest = "MatchMaking:KafkaTopics:Request";
                public const string MatchMakingComplete = "MatchMaking:KafkaTopics:Complete";
            }

            public static class RateLimiter
            {
                public const string MillisecondsLimit = "MatchMaking:RateLimiter:MillisecondsLimit";
                public const string RequestsCountLimit = "MatchMaking:RateLimiter:RequestsCountLimit";
                public const string RequestsQueueLimit = "MatchMaking:RateLimiter:RequestsQueueLimit";
                public const string IsActiveGlobally = "MatchMaking:RateLimiter:IsActiveGlobally";
                public const string IsActiveForMatchSearch = "MatchMaking:RateLimiter:IsActiveForMatchSearch";
                public const string IsActiveForMatchStatus = "MatchMaking:RateLimiter:IsActiveForMatchStatus";
            }
        }
    }
}