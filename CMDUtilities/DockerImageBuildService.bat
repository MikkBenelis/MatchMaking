@echo off
cd /d "%~dp0"
cd ..

docker build -t match_making_service:latest^
 -f "MatchMakingService\MatchMakingServiceAPI\Dockerfile"^
 .

pause