namespace MatchMakingWorker.Data;

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

        public const string MatchUserConsumed = "Consumed match request for UserID={UserID}!";
        public const string MatchUserConsumedAlready = "UserID={UserID} has already been consumed!";
        public const string MatchUserConsumptionError = "Match request consumption failed!";

        public const string MatchResultProduced = "Produced match result for MatchID={MatchID}!";
        public const string MatchResultProducedDetails = "[{i}/{c}] Produced match result UserID={UserID}";
        public const string MatchResultProductionError = "Match result production failed for MatchID={MatchID}!";
    }

    public static class Configuration
    {
        public static class ConnectionStrings
        {
            public const string KafkaName = "Kafka";
            public const string RedisName = "Redis";
        }

        public static class MatchMaking
        {
            public static class KafkaTopics
            {
                public const string MatchMakingRequestKey = "MatchMaking:KafkaTopics:Request";
                public const string MatchMakingCompleteKey = "MatchMaking:KafkaTopics:Complete";
            }

            public static class RedisLogging
            {
                public const string IsActiveKey = "MatchMaking:RedisLogging:IsActive";
                public const string LifetimeHoursKey = "MatchMaking:RedisLogging:LifetimeHours";
            }

            public const string GroupSizeKey = "MatchMaking:GroupSize";
        }
    }
}