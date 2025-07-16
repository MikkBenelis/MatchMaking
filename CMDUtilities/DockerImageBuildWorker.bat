@echo off
cd /d "%~dp0"
cd ..

docker build -t match_making_worker:latest^
 -f "MatchMakingWorker\MatchMakingWorkerAPP\Dockerfile"^
 .

pause