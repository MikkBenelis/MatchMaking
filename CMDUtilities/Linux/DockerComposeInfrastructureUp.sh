#!/bin/sh

cd "$(dirname "$0")" \
 && cd "$(pwd -P)" \
 || exit 1
cd ../..

COMPOSE_FILE="DockerComposeInfrastructure.yml"
COMPOSE_FILE_PATH="DockerCompose/${COMPOSE_FILE}"

echo "[36mStarting match making infrastructure...[31m"

docker compose \
 -f "${COMPOSE_FILE_PATH}" \
 up -d

echo "[32mStarting match making infrastructure... Done![0m"

echo "[33m"

echo "Press any key to continue . . ."
stty -icanon -echo # Limit terminal input
: key="$(dd bs=1 count=1 2>/dev/null)"
stty icanon echo # Reset terminal input

echo "[0m"