# STAGE 1: Publish app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS publish
ARG BUILD_CONFIGURATION=Release

WORKDIR /src
COPY . .

WORKDIR "/src/MatchMakingWorker/MatchMakingWorkerAPP"
RUN dotnet restore "./MatchMakingWorkerAPP.csproj"
RUN dotnet publish "./MatchMakingWorkerAPP.csproj" \
    -c $BUILD_CONFIGURATION -o /app/publish \
    /p:UseAppHost=false

# STAGE 2: Setup app runtime

FROM mcr.microsoft.com/dotnet/runtime:9.0 AS runtime
ARG APP_UID=1000
USER $APP_UID
WORKDIR /app

COPY --from=publish /app/publish .
ENTRYPOINT [ "dotnet", "MatchMakingWorkerAPP.dll" ]