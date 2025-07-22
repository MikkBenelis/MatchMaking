#!/bin/sh

cd "$(dirname "$0")" \
 && cd "$(pwd -P)" \
 || exit 1
cd ../..

echo "[36mBuilding 'match_making_service' image...[31m"

docker build -t match_making_service:latest \
 -f "MatchMakingService/MatchMakingServiceAPI/Dockerfile" \
 --target=runtime .

echo "[32mBuilding 'match_making_service' image... Done![0m"

echo "[33m"

echo "Press any key to continue . . ."
stty -icanon -echo # Limit terminal input
: key="$(dd bs=1 count=1 2>/dev/null)"
stty icanon echo # Reset terminal input

echo "[0m"