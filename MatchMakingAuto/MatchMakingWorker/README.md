# MatchMakingWorker

---

## Description

- Requires running `Kafka` broker (you can use `../DockerCompose.yml` to run it)
- This project contains automatic **MatchMakingWorker** implementation for testing purposes:
	- It consumes `matchmaking.request` Kafka topic when its queue is not empty
	- It produces `matchmaking.complete` Kafka topic when there is enough users (configurable)
	- Both topics have `1` partition and its replication factor is set to `1` (configurable)


- Entry point file - `MatchMakingWorker.cs`
- Configuration file - `MatchMakingWorker.json`
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

### `MatchMaking:GroupSize`:

- Group size of a match
- Default value: `3`