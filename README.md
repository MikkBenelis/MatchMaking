# MatchMaking - ArtStudio Test Task

---

## Repository contents

### MatchMakingAuto

- Not used by the actual **MatchMaking** solutions
- Automated minimal **MatchMaking** solution (Kafka-only)
- Originally was used to implement Kafka-based communication
- More information can be found in its own **README.md** file

### MatchMakingService

- **MatchMaking** service (**HTTP API** + users producer and matches consumer)
- More information can be found in its own **README.md** file

### MatchMakingWorker

- **MatchMaking** worker (users consumer and matches producer)
- More information can be found in its own **README.md** file

---

## Common information

- Contains two .NET 9.0 solutions: **MatchMakingService** and **MatchMakingWorker**
- Configured to work best with  **JetBrains Rider IDE** (code inspection and publish profiles)
- Uses **Kafka** as an event streaming platform and **Redis** as an in-memory DB
- Utilizes **Docker** for **build** process and **runtime** configuration

---

## Prerequisites

- Running **Docker** with the following list of optionally pre-pulled images:
	- `apache/kafka:latest` to run Kafka as an event streaming platform
	- `provectuslabs/kafka-ui:latest` to inspect Kafka event stream
	- `redis:latest` to run Redis as an in-memory database
	- `redis/redisinsight:latest` to inspect Redis in-memory database
	- `mcr.microsoft.com/dotnet/sdk:9.0` to build **.NET** solutions in **Docker**
	- `mcr.microsoft.com/dotnet/aspnet:9.0` to run **MatchMakingService** app
	- `mcr.microsoft.com/dotnet/runtime:9.0` to run **MatchMakingWorker** app
- Host TCP ports need to be available:
	- `800` - to run HTTP API
	- `4430` - to run HTTPS API
	- `5540` - to access RedisInsight
	- `6379` - to access Redis DB
	- `8082` - to access Kafka UI
	- `9092` - to access Kafka

---

## Configuration

### MatchMaking.Service Configuration:

- Configuration file: `MatchMakingService/MatchMakingServiceAPI/MatchMakingServiceAPI.json`
- Configuration description: `MatchMakingService/MatchMakingServiceAPI/README.md`

### MatchMaking.Worker Configuration:

- Configuration file: `MatchMakingWorker/MatchMakingWorkerAPP/MatchMakingWorkerAPP.json`
- Configuration description: `MatchMakingWorker/MatchMakingWorkerAPP/README.md`

---

## Startup instructions

### TL;DR (OR FOLLOW STEPS 1-7 MANUALLY)

- Run `CMDUtilities/SingleFileStartup.bat` to build, setup and start everything
- Run `CMDUtilities/SingleFileCleanup.bat` to stop and remove everything
- Use SwaggerUI to test API endpoints at http://localhost:800 or https://localhost:4430
- **[OPTIONAL]** Install SSL certificate (client-only):
	- It is only required for the HTTPS support (can be toggled via configuration file)
	- Certificate: `MatchMaking.Service/MatchMakingServiceAPI/MatchMakingServiceAPI.pfx`
	- The provided certificate password is `default`

### Step 1: Build MatchMakingService Image

- Run `CMDUtilities/DockerImageBuildService.bat`
- The `match_making_service:latest` docker image will be produced

### Step 2: Build MatchMakingWorker Image

- Run `CMDUtilities/DockerImageBuildWorker.bat`
- The `match_making_worker:latest` image will be produced

### Step 3: Setup MatchMaking Infrastructure

Run `CMDUtilities/DockerComposeInfrastructureUp.bat`

- The `match_making` network will be created
- The `match_making` volume will be created
- The following containers will be created:
	- `match_making_kafka_ui` based on `provectuslabs/kafka-ui:latest` listening `8080` host port
	- `match_making_kafka` based on `apache/kafka:latest` listening `9092` host port
	- `match_making_redis_insight` based on `redis/redisinsight:latest` listening `5540` host port
	- `match_making_redis` based on `redis:latest` listening `6379` host port

### Step 4: Init Kafka Configuration

Run `CMDUtilities/KafkaInitialSetup.bat`

- The `matchmaking.request` topic with **two partitions** will be created
- The `matchmaking.complete` topic with **single partition** will be created

### Step 5: Setup MatchMaking Payload

Run `CMDUtilities/DockerComposePayloadUp.bat`

- The following containers will be created:
	- `match_making_service` based on `match_making_service:latest` listening `800` and `4430` host ports
	- `match_making_worker_1` and `match_making_worker_2` based on `match_making_worker:latest`

### Step 6: Install SSL certificate [OPTIONAL]

If you are going to test it with SSL/HTTPS enabled:

- certificate needs to be installed on a client machine
- certificate: `MatchMaking.Service/MatchMakingServiceAPI/MatchMakingServiceAPI.pfx`
- the provided certificate password is `default`

### Step 7: Test Open SwaggerUI to test API endpoints

- Swagger UI HTTP API is available at http://localhost:800/swagger
- Swagger UI HTTPS API is available at https://localhost:4430/swagger
- RedisInsight is available at http://localhost:5540
- Kafka UI is available at http://localhost:8082

### Cleanup After testing

- Run `CMDUtilities/DockerComposePayloadDown.bat`
- Run `CMDUtilities/DockerComposeInfrastructureDown.bat`
- Run `CMDUtilities/DockerArtifactsCleanup.bat`

---

## What can be improved / TODO list

- Add Linux batch command files (`.sh` alternative to `.bat` files)
- Improve application security (add authentication, change defaults, etc.)
- Handle configuration errors (missing/invalid values, default values, etc.)
- Handle docker compose related errors (port availability, existing image/container, etc.)
- Handle user disconnection and match completion logic (remove from history/queue, etc.)
- Use better logging options (log matches and players in more structured way, etc.)
- Use some sort of automapper to map between DTOs and models

---