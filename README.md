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

### TBD

---

## Startup instructions

### TBD