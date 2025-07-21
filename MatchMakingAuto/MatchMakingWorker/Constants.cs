namespace MatchMakingWorker;

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

        public const string MatchUserConsumed = "Consumed match request for UserID={UserID}!";
        public const string MatchUserConsumedAlready = "UserID={UserID} has already been consumed!";
        public const string MatchUserConsumptionError = "Match request consumption failed!";

        public const string MatchResultProduced = "Produced match result for MatchID={MatchID}!";
        public const string MatchResultProducedDetails = "[{i}/{c}] Produced match result UserID={UserID}";
        public const string MatchResultProductionError = "Match result production failed for MatchID={MatchID}!";

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

            internal const string GroupSize = "MatchMaking:GroupSize";
        }
    }
}