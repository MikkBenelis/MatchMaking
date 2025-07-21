@echo off
cd /d "%~dp0"
cd ..

set COMPOSE_FILE=DockerComposePayload.yml
set COMPOSE_FILE_PATH=DockerCompose/%COMPOSE_FILE%

docker compose^
 -f "%COMPOSE_FILE_PATH%"^
 up -d

pause