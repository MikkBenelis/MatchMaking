@echo off
cd /d "%~dp0"
cd ../..

set COMPOSE_FILE=DockerComposePayload.yml
set COMPOSE_FILE_PATH=DockerCompose/%COMPOSE_FILE%

echo [36mStarting match making payload...[31m

docker compose^
 -f "%COMPOSE_FILE_PATH%"^
 up -d

echo [32mStarting match making payload... Done![0m

echo [33m

pause

echo [0m