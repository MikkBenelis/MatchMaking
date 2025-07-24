@echo off

echo [36mRemoving 'match_making_service' image...[31m
docker image rm match_making_service
echo [32mRemoving 'match_making_service' image... Done![0m

echo.

echo [36mRemoving 'match_making_worker' image...[31m
docker image rm match_making_worker
echo [32mRemoving 'match_making_worker' image... Done![0m

echo.

echo [36mRemoving 'match_making' volume...[31m
docker volume rm match_making
echo [32mRemoving 'match_making' volume... Done![0m

echo.

echo [36mRemoving 'match_making' network...[31m
docker network rm match_making
echo [32mRemoving 'match_making' network... Done![0m

echo [33m

pause

echo [0m