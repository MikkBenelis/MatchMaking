# STAGE 1: Publish app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS publish
ARG BUILD_CONFIGURATION=Release

WORKDIR /src
COPY [ ".editorconfig", \
	"Directory.Build.props", \
	"Directory.Build.targets", \
	"global.json", \
	"./" ]

COPY "MatchMakingWorker/MatchMaking.Worker.sln" "./"
COPY "MatchMakingWorker/MatchMakingWorker.Data/MatchMakingWorker.Data.csproj" "./MatchMakingWorker.Data/"
COPY "MatchMakingWorker/MatchMakingWorker.Services/MatchMakingWorker.Services.csproj" "./MatchMakingWorker.Services/"
COPY "MatchMakingWorker/MatchMakingWorkerAPP/MatchMakingWorkerAPP.csproj" "./MatchMakingWorkerAPP/"

RUN dotnet restore

COPY "MatchMakingWorker" "./"
WORKDIR "/src/MatchMakingWorkerAPP"

RUN dotnet publish "./MatchMakingWorkerAPP.csproj" \
    -c $BUILD_CONFIGURATION -o /app \
    -r linux-x64 /p:UseAppHost=false

# STAGE 2: Setup app runtime

FROM mcr.microsoft.com/dotnet/runtime:9.0 AS runtime
ARG APP_UID=1000
USER $APP_UID
WORKDIR /app

COPY --from=publish /app .
ENTRYPOINT [ "dotnet", "MatchMakingWorkerAPP.dll" ]