@echo off
if [%1%2%3]==[] (
    echo Usage: %~nx0 command
    echo Example: %~nx0 devices
    echo Example: %~nx0 disconnect
    exit /b 1
)

REM chcp
CHCP 65001

set command=%*
set ADB=adb

echo %%1 command: %command%


echo where adb:
where %ADB%
if %ERRORLEVEL% NEQ 0 (
    echo Error: adb is not installed! %%ADB%%: %ADB% >&2
    exit /b 1
)


:: check adb connection
adb get-state
if errorlevel 1 (
    echo No connected devices>&2
    exit /b 1
)

:: execute
echo call: adb %command% ...
adb %command%

echo Finish.
