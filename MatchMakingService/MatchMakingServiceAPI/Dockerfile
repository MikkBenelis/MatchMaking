# STAGE 1: Publish app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS publish
ARG BUILD_CONFIGURATION=Release

WORKDIR /src
COPY . .

WORKDIR "/src/MatchMakingService/MatchMakingServiceAPI"
RUN dotnet restore "./MatchMakingServiceAPI.csproj"
RUN dotnet publish "./MatchMakingServiceAPI.csproj" \
    -c $BUILD_CONFIGURATION -o /app/publish \
    /p:UseAppHost=false

# STAGE 2: Setup app runtime

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
ARG APP_UID=1000
USER $APP_UID
WORKDIR /app

EXPOSE 800
EXPOSE 4430

COPY --from=publish /app/publish .
ENTRYPOINT [ "dotnet", "MatchMakingServiceAPI.dll" ]