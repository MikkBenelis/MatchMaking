@echo off

set KAFKA_TOPICS=/opt/kafka/bin/kafka-topics.sh
set BROKER=host.docker.internal:9092

docker exec -t match_making_kafka sh -c^
 "%KAFKA_TOPICS% --bootstrap-server %BROKER% --create --topic matchmaking.request --partitions 2"

docker exec -t match_making_kafka sh -c^
 "%KAFKA_TOPICS% --bootstrap-server %BROKER% --create --topic matchmaking.complete"

pause