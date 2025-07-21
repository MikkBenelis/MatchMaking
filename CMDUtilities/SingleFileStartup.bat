@echo off

call "%~dp0/DockerImageBuildService.bat"
call "%~dp0/DockerImageBuildWorker.bat"

call "%~dp0/DockerComposeInfrastructureUp.bat"
call "%~dp0/KafkaInitialSetup.bat"
call "%~dp0/DockerComposePayloadUp.bat"