@echo off
cd /d "%~dp0"
cd ../..

set COMPOSE_FILE=DockerComposeInfrastructure.yml
set COMPOSE_FILE_PATH=DockerCompose/%COMPOSE_FILE%

echo [36mStopping match making infrastructure...[31m

docker compose^
 -f "%COMPOSE_FILE_PATH%"^
 down

echo [32mStopping match making infrastructure... Done![0m

echo [33m

pause

echo [0m