@echo off

echo [36mRunning single file cleanup...[31m

echo [33m

pause

echo [0m

call "%~dp0/DockerComposePayloadDown.bat"
call "%~dp0/DockerComposeInfrastructureDown.bat"
call "%~dp0/DockerArtifactsCleanup.bat"

echo [32mRunning single file cleanup... Done![0m

echo [33m

pause

echo [0m