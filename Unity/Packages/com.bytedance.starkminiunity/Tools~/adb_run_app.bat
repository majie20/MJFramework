@echo off
if [%1%2%3]==[] (
    echo Usage: %~nx0 pkg_name
    echo Example: %~nx0 com.ss.android.ugc.aweme
    echo Example: %~nx0 com.ss.android.article.news
    exit /b 1
)

REM chcp
CHCP 65001

set pkg_name=%1
set ADB=adb

echo %%1 pkg_name: %pkg_name%
REM echo ADB: %ADB%

if [%pkg_name%]==[] (
    echo pkg_name not valid! %%pkg_name%%: %pkg_name%
    exit /b 1
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
echo call: adb shell monkey -p %pkg_name% -c android.intent.category.LAUNCHER 1 ...
adb shell monkey -p %pkg_name% -c android.intent.category.LAUNCHER 1


echo Finish.
