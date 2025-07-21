# MatchMaking - Service API

---

## Description

- This solution contains **MatchMakingService** implementation:
	- It produces `matchmaking.request` Kafka topic (bound to API endpoint)
	- It consumes `matchmaking.complete` Kafka topic when its queue is not empty
	- It writes logs to both console and Redis database for quick inspection
- Requires running `Kafka` and `Redis` to function


- Entry point file - `MatchMakingServiceAPI.cs`
- Configuration file - `MatchMakingServiceAPI.json`
- Application can be easily built as a single file

---

## Configuration

### `ConnectionStrings:Kafka`:

- Kafka message broker bootstrap servers
- Default value: `"host.docker.internal:9092"`

### `ConnectionStrings:Redis`:

- Redis in-memory database server
- Default value: `"host.docker.internal:9379"`

### `AllowSwagger`:

- Is SwaggerUI enabled?
- Default value: `true`

### `ForceHTTPSRedirect`

- Should an API redirect to HTTPS?
- Default value: `false`

### `MatchMaking:KafkaTopics:Request`:

- Kafka topic name for user match requests
- Default value: `"matchmaking.request"`

### `MatchMaking:KafkaTopics:Complete`:

- Kafka topic name for completed matches
- Default value: `"matchmaking.complete"`

### `MatchMaking:RateLimiter:MillisecondsLimit`:

- Min delay between per IP requests
- Uses default fixed window rate limiter
- Default value: `100` (in milliseconds)

### `MatchMaking:RateLimiter:RequestsCountLimit`:

- Number of requests before starting the rate limiter
- Default value: `1` (number of requests per IP)

### `MatchMaking:RateLimiter:RequestsQueueLimit`:

- Requests queue size to hold before rejecting any extra
- Default value: `0` (`0` means reject any extra request)

### `MatchMaking:RateLimiter:IsActiveGlobally`:

- Should the rate limiter be enabled?
- Default value: `true`

### `MatchMaking:RateLimiter:IsActiveForMatchSearch`:

- Use rate limiter for the `/matches/search` endpoint?
- Default value: `true`

### `MatchMaking:RateLimiter:IsActiveForMatchStatus`:

- Use rate limited for the `/matches/status` endpoint?
- Default value: `false`