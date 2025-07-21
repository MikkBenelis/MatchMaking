@echo off
cd /d "%~dp0"

docker rmi match_making_service
docker rmi match_making_worker

docker volume rm match_making
::docker network rm match_making

pause