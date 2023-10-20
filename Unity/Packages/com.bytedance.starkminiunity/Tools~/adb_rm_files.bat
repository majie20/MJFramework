@echo off
if [%1%2%3]==[] (
    echo Usage: %~nx0 filepath
    echo Example: %~nx0 /sdcard/Android/data/com.ss.android.ugc.aweme/files/_ucfiles
    echo Example 2 folders: %~nx0 /sdcard/Android/data/folder1 /sdcard/Android/data/folder2
    exit /b 1
)

REM chcp
CHCP 65001

set filepath=%~1
set filepath2=%~2
set filepath3=%~3
set ADB=adb

set LEN_THRESH=10
echo %%1 filepath:  %filepath%
if [%filepath%]==[] (
    echo filepath not valid! %%filepath%%: %filepath%
    exit /b 1
)
call :strLen filepath filepathlen
if %filepathlen% LSS %LEN_THRESH% (
    echo filepath length not valid! too short: %filepathlen% ^< %LEN_THRESH%! filepath: %filepath%
    exit /b 1
)

call :strLen filepath2 filepath2len
if not [%~2]==[] (
    echo %%2 filepath2: %filepath2%
    if %filepath2len% LSS %LEN_THRESH% (
        echo filepath2 length not valid! too short: %filepath2len% ^< %LEN_THRESH%! filepath2: %filepath2%
        exit /b 1
    )
)
call :strLen filepath3 filepath3len
if not [%~3]==[] (
    echo %%3 filepath3: %filepath3%
    if %filepath3len% LSS %LEN_THRESH% (
        echo filepath3 length not valid! too short: %filepath3len% ^< %LEN_THRESH%! filepath3: %filepath3%
        exit /b 1
    )
)

echo where adb:
where %ADB%
if %ERRORLEVEL% NEQ 0 (
    echo Error: adb is not installed! %%ADB%%: %ADB% >&2
    exit /b 1
)



:: check adb connection
adb get-state

:: execute
echo call: adb shell rm -rf "%filepath%" ...
adb shell rm -rf "%filepath%"
if not [%~2]==[] (
    echo call: adb shell rm -rf "%~2" ...
    adb shell rm -rf "%~2"
)
if not [%~3]==[] (
    echo call: adb shell rm -rf "%~3" ...
    adb shell rm -rf "%~3"
)


echo Finish.
exit /b

:strLen
setlocal enabledelayedexpansion

:strLen_Loop
   if "!%1!"=="" endlocal & set %2=0 & goto :eof
   if not "!%1:~%len%!"=="" set /A len+=1 & goto :strLen_Loop
(endlocal & set %2=%len%)
goto :eof
