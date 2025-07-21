# MatchMakingService

---

## Description

- This project contains automatic **MatchMakingService**:
	- It produces `matchmaking.request` Kafka topic every `3` to `5` seconds (configurable)
	- It consumes `matchmaking.complete` Kafka topic when its queue is not empty
	- Both topics have `1` partition and its replication factor is set to `1`
- Requires running `Kafka` (you can use `DockerCompose.yml` to start it)


- Entry point file - `MatchMakingService.cs`
- Configuration file - `MatchMakingService.json`
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

### `MatchMaking:RequestDelay:MinMS`:

- Min delay between sending match request
- Default value: `3000` (in milliseconds)

### `MatchMaking:RequestDelay:MaxMS`:

- Max delay between sending match request
- Default value: `5000` (in milliseconds)