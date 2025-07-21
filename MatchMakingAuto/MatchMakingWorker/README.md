# MatchMakingWorker

---

## Description

- This project contains automatic **MatchMakingWorker**:
	- It consumes `matchmaking.request` Kafka topic when its queue is not empty
	- It produces `matchmaking.complete` Kafka topic when there is enough users
	- Both topics have `1` partition and its replication factor is set to `1`
- Requires running `Kafka` (you can use `DockerCompose.yml` to start it)


- Entry point file - `MatchMakingWorker.cs`
- Configuration file - `MatchMakingWorker.json`
- Application can be easily built as a single file

---

## Configuration

### `ConnectionStrings:Kafka`:

- Kafka message broker bootstrap servers
- Default value: `"host.docker.internal:9092"`

### `MatchMaking:KafkaTopics:Request`:

- Kafka topic name for user match requests
- Default value: `"matchmaking.request"`

### `MatchMaking:KafkaTopics:Complete`:

- Kafka topic name for completed matches
- Default value: `"matchmaking.complete"`

### `MatchMaking:GroupSize`:

- Group size of a match
- Default value: `3`