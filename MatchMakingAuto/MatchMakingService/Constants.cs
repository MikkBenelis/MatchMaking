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
            internal const string KafkaName = "Kafka";
        }

        internal static class MatchMaking
        {
            internal static class KafkaTopics
            {
                internal static class MatchMakingRequest
                {
                    internal const string NameKey = "MatchMaking:KafkaTopics:Request:Name";
                    internal const string NumPartitionsKey = "MatchMaking:KafkaTopics:Request:NumPartitions";
                    internal const string ReplicationFactorKey = "MatchMaking:KafkaTopics:Request:ReplicationFactor";
                }

                internal static class MatchMakingComplete
                {
                    internal const string NameKey = "MatchMaking:KafkaTopics:Complete:Name";
                    internal const string NumPartitionsKey = "MatchMaking:KafkaTopics:Complete:NumPartitions";
                    internal const string ReplicationFactorKey = "MatchMaking:KafkaTopics:Complete:ReplicationFactor";
                }
            }

            internal static class RequestDelay
            {
                internal const string MinMSKey = "MatchMaking:RequestDelay:MinMS";
                internal const string MaxMSKey = "MatchMaking:RequestDelay:MaxMS";
            }
        }
    }
}