@echo off
if [%1%2%3]==[] (
    echo Usage: %~nx0 apk_path [pkg_name]
    echo Example: %~nx0 douyin.apk com.ss.android.ugc.aweme
    echo Example: %~nx0 toutiao.apk com.ss.android.article.news
    exit /b 1
)

REM chcp
CHCP 65001

set apk_path=%1
set pkg_name=%2
set ADB=adb

echo %%1 apk_path: %apk_path%
echo %%2 pkg_name: %pkg_name%
REM echo ADB: %ADB%

if [%apk_path%]==[] (
    echo apk_path not valid! %%apk_path%%: %apk_path%
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

:: install
echo call: adb install -r %apk_path% ...
adb install -r %apk_path%


:: run app
if NOT [%pkg_name%]==[] (
    echo call: adb shell monkey -p %pkg_name% ...
    adb shell monkey -p %pkg_name% -c android.intent.category.LAUNCHER 1
) else (
    echo %%2 %%pkg_name%% is empty, skip adb shell monkey.
)

echo Finish.
