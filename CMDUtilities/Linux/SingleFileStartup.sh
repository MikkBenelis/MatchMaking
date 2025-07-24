#!/bin/sh

cd "$(dirname "$0")" \
 && cd "$(pwd -P)" \
 || exit 1

PWD=$(pwd -P)

echo "[36mRunning single file startup...[31m"

echo "[33m"

echo "Press any key to continue . . ."
stty -icanon -echo # Limit terminal prints
: key="$(dd bs=1 count=1 2>/dev/null)"
stty icanon echo # Reset terminal

echo "[0m"

sh "${PWD}/DockerImageBuildService.sh"
sh "${PWD}/DockerImageBuildWorker.sh"

sh "${PWD}/DockerComposeInfrastructureUp.sh"
sh "${PWD}/KafkaInitialSetup.sh"
sh "${PWD}/DockerComposePayloadUp.sh"

echo "[32mRunning single file startup... Done![0m"

echo "[33m"

echo "Press any key to continue . . ."
stty -icanon -echo # Limit terminal input
: key="$(dd bs=1 count=1 2>/dev/null)"
stty icanon echo # Reset terminal input

echo "[0m"