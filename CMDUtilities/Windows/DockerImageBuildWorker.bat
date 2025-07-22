@echo off
cd /d "%~dp0"
cd ../..

SET DOCKER_CLI_HINTS=FALSE

echo [36mBuilding 'match_making_worker' image...[31m

docker buildx build -t match_making_worker:latest^
 -f "MatchMakingWorker/MatchMakingWorkerAPP/Dockerfile"^
 --target=runtime .

echo [32mBuilding 'match_making_worker' image... Done![0m

echo [33m

pause

echo [0m