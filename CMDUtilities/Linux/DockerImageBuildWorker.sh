#!/bin/sh

cd "$(dirname "$0")" \
 && cd "$(pwd -P)" \
 || exit 1
cd ../..

echo "[36mBuilding 'match_making_worker' image...[31m"

docker build -t match_making_worker:latest \
 -f "MatchMakingWorker/MatchMakingWorkerAPP/Dockerfile" \
 --target=runtime .

echo "[32mBuilding 'match_making_worker' image... Done![0m"

echo "[33m"

echo "Press any key to continue . . ."
stty -icanon -echo # Limit terminal input
: key="$(dd bs=1 count=1 2>/dev/null)"
stty icanon echo # Reset terminal input

echo "[0m"