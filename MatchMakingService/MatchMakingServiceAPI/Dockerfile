# STAGE 1: Publish app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS publish
ARG BUILD_CONFIGURATION=Release

WORKDIR /src
COPY [ ".editorconfig", \
	"Directory.Build.props", \
	"Directory.Build.targets", \
	"global.json", \
	"./" ]

COPY "MatchMakingService/MatchMaking.Service.sln" "./"
COPY "MatchMakingService/MatchMakingService.Data/MatchMakingService.Data.csproj" "./MatchMakingService.Data/"
COPY "MatchMakingService/MatchMakingService.Services/MatchMakingService.Services.csproj" "./MatchMakingService.Services/"
COPY "MatchMakingService/MatchMakingServiceAPI/MatchMakingServiceAPI.csproj" "./MatchMakingServiceAPI/"

RUN dotnet restore

COPY "MatchMakingService" "./"
WORKDIR "/src/MatchMakingServiceAPI"

RUN dotnet publish "./MatchMakingServiceAPI.csproj" \
    -c $BUILD_CONFIGURATION -o /app \
    -r linux-x64 /p:UseAppHost=false

# STAGE 2: Setup app runtime

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
ARG APP_UID=1000
USER $APP_UID
WORKDIR /app

EXPOSE 800
EXPOSE 4430

COPY --from=publish /app .
ENTRYPOINT [ "dotnet", "MatchMakingServiceAPI.dll" ]