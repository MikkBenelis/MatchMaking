# MatchMakingService

---

## Description

- Requires running `Kafka` broker (you can use `../DockerCompose.yml` to run it)
- This project contains automatic **MatchMakingService** implementation for testing purposes:
	- It produces `matchmaking.request` Kafka topic every `3` to `5` seconds (configurable)
	- It consumes `matchmaking.complete` Kafka topic when its queue is not empty
	- Both topics have `1` partition and its replication factor is set to `1` (configurable)


- Entry point file - `MatchMakingService.cs`
- Configuration file - `MatchMakingService.json`
- Project can be easily published as a single file app

---

## Configuration

### `ConnectionStrings:Kafka`:

- Kafka message broker bootstrap servers
- Default value: `"host.docker.internal:9092"`

### `MatchMaking:KafkaTopics:Request:Name`:

- Kafka topic name for user match requests
- Default value: `"matchmaking.request"`

### `MatchMaking:KafkaTopics:Request:NumPartitions`:

- Match requests topic number of partitions
- Default value: `1`

### `MatchMaking:KafkaTopics:Request:ReplicationFactor`:

- Match requests topic replication factor
- Default value: `1`

### `MatchMaking:KafkaTopics:Complete:Name`:

- Kafka topic name for completed matches
- Default value: `"matchmaking.complete"`

### `MatchMaking:KafkaTopics:Complete:NumPartitions`:

- Completed matches topic number of partitions
- Default value: `1`

### `MatchMaking:KafkaTopics:Complete:ReplicationFactor`:

- Completed matches topic replication factor
- Default value: `1`

### `MatchMaking:RequestDelay:MinMS`:

- Min delay between sending match request
- Default value: `3000` (in milliseconds)

### `MatchMaking:RequestDelay:MaxMS`:

- Max delay between sending match request
- Default value: `5000` (in milliseconds)