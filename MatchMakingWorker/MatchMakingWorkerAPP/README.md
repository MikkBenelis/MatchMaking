# MatchMaking - Worker App

---

## Description

- This solution contains **MatchMakingWorker** implementation:
	- It consumes `matchmaking.request` Kafka topic when its queue is not empty
	- It produces `matchmaking.complete` Kafka topic there is enough users
	- It writes logs to both console and Redis database for quick inspection
- Requires running `Kafka` and `Redis` to function


- Entry point file - `MatchMakingWorkerAPP.cs`
- Configuration file - `MatchMakingWorkerAPP.json`
- Application can be easily built as a single file

---

## Configuration

### `ConnectionStrings:Kafka`:

- Kafka message broker bootstrap servers
- Default value: `"host.docker.internal:9092"`

### `ConnectionStrings:Redis`:

- Redis in-memory database server
- Default value: `"host.docker.internal:9379"`

### `MatchMaking:KafkaTopics:Request`:

- Kafka topic name for user match requests
- Default value: `"matchmaking.request"`

### `MatchMaking:KafkaTopics:Complete`:

- Kafka topic name for completed matches
- Default value: `"matchmaking.complete"`

### `MatchMaking:GroupSize`:

- Group size of a match
- Default value: `3`