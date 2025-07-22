@echo off

set KAFKA_TOPICS=/opt/kafka/bin/kafka-topics.sh
set BROKER=host.docker.internal:9092

echo [36mCreating 'matchmaking.request' Kafka topic...[31m

docker exec -t match_making_kafka sh -c^
 "%KAFKA_TOPICS% --bootstrap-server %BROKER% --create --topic matchmaking.request --partitions 2"

echo [32mCreating 'matchmaking.request' Kafka topic... Done![0m

echo [36mCreating 'matchmaking.complete' Kafka topic...[31m

docker exec -t match_making_kafka sh -c^
 "%KAFKA_TOPICS% --bootstrap-server %BROKER% --create --topic matchmaking.complete"

echo [32mCreating 'matchmaking.complete' Kafka topic... Done![0m

echo [33m

pause

echo [0m