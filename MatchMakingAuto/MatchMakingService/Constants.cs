namespace MatchMakingService;

internal static class Constants
{
    internal static class LogMessages
    {
        internal const string ServiceStarted = "Service started!";
        internal const string ServiceRunning = "Service is running!";
        internal const string ServiceWorking = "Service is working...";
        internal const string ServiceStopping = "Service stopping...";
        internal const string ServiceStopped = "Service stopped!";

        public const string FailureReason = "Failure reason: {ErrorReason}";

        public const string MatchUserProduced = "Produced match request for UserID={UserID}!";
        public const string MatchUserProductionError = "Match request production failed for UserID={UserID}!";

        public const string MatchResultConsumed = "Consumed match result for MatchID={MatchID}!";
        public const string MatchResultConsumedDetails = "[{i}/{c}] Consumed match result UserID={UserID}";
        public const string MatchResultConsumptionError = "Match result consumption failed!";

        public const string TopicCreationError = "An error occured creating topic!";
    }

    internal static class Configuration
    {
        internal static class ConnectionStrings
        {
            internal const string Kafka = "Kafka";
        }

        internal static class MatchMaking
        {
            internal static class KafkaTopics
            {
                internal const string MatchMakingRequest = "MatchMaking:KafkaTopics:Request";
                internal const string MatchMakingComplete = "MatchMaking:KafkaTopics:Complete";
            }

            internal static class RequestDelay
            {
                internal const string MinMS = "MatchMaking:RequestDelay:MinMS";
                internal const string MaxMS = "MatchMaking:RequestDelay:MaxMS";
            }
        }
    }
}