@echo off

call "%~dp0/DockerComposePayloadDown.bat"
call "%~dp0/DockerComposeInfrastructureDown.bat"
call "%~dp0/DockerArtifactsCleanup.bat"