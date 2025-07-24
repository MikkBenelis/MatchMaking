@echo off

echo [36mRunning single file startup...[31m

echo [33m

pause

echo [0m

call "%~dp0/DockerImageBuildService.bat"
call "%~dp0/DockerImageBuildWorker.bat"

call "%~dp0/DockerComposeInfrastructureUp.bat"
call "%~dp0/KafkaInitialSetup.bat"
call "%~dp0/DockerComposePayloadUp.bat"

echo [32mRunning single file startup... Done![0m

echo [33m

pause

echo [0m