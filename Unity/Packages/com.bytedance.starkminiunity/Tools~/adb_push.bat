@echo off
if [%1%2%3]==[] (
    echo Usage: %~nx0 apk_path push_to_path
    echo Example: %~nx0 miniapp.apk /sdcard/Android/data/com.ss.android.ugc.aweme/files/.patchs/
    echo Example: %~nx0 appbrandplugin.apk /sdcard/Android/data/com.ss.android.article.news/files/.patchs/
    exit /b 1
)

REM chcp
CHCP 65001

set apk_path=%1
set push_to_path=%2
set ADB=adb

echo %%1 apk_path: %apk_path%
echo %%2 push_to_path: %push_to_path%
REM echo ADB: %ADB%

set apk_path=%apk_path:"=%
set push_to_path=%push_to_path:"=%

if "%apk_path%" == "" (
    echo apk_path not valid! %%apk_path%%: %apk_path%
    exit /b 1
)

if "%push_to_path%" == "" (
    echo push_to_path not valid! %%push_to_path%%: %push_to_path%
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
set push_path_last=%push_to_path:~-9%
set push_to_dir=%push_to_path:~0,-9%
if "%push_path_last%"=="/main.apk" (
    echo push_to_dir: %push_to_dir%
    echo Push file is "/main.apk". Clean local dir first ...
    echo adb shell "rm -rf %push_to_dir%"
    adb shell "rm -rf %push_to_dir%"
)
:: execute
echo push_to_path: %push_to_path%
echo call: adb push "%apk_path%" "%push_to_path%" ...
adb push "%apk_path%" "%push_to_path%"


echo Finish.
