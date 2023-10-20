@echo off
if [%1%2%3]==[] (
    echo Usage: %~nx0 host_type app_id [version_type]
    echo @Param %%1 host_type options: [ douyin, toutiao ]
    echo @Param %%2 app_id
    echo @Param %%3 Optional, version_type options: [ "" as preview, preview, latest, current, local ]
    echo Example: %~nx0 douyin tt9a4aecf7057074ae latest
    exit /b 1
)

REM chcp
CHCP 65001

set host_type=%1
set app_id=%2
set version_type=%3
set launch_from_local=0

if [%version_type%]==[] (
    set version_type=preview
)
if [%version_type%]==[local] (
    set version_type=latest
    set launch_from_local=1
)

set ADB=adb

echo %%1 host_type: %host_type%
echo %%2 app_id: %app_id%
echo %%3 version_type: %version_type%
REM echo ADB: %ADB%

if [%host_type%]==[] (
    echo host_type not valid! %%host_type%%: %host_type%
    exit /b 1
)

if [%app_id%]==[] (
    echo app_id not valid! %%app_id%%: %app_id%
    exit /b 1
)

if [%version_type%]==[] (
    echo version_type not valid! %%version_type%%: %version_type%
    exit /b 1
)

echo where adb:
where %ADB%
if %ERRORLEVEL% NEQ 0 (
    echo Error: adb is not installed! %%ADB%%: %ADB% >&2
    exit /b 1
)

set schemaName=sslocal
set pkg_name=com.ss.android.ugc.aweme
if [%host_type%]==[douyin] (
    set schemaName=snssdk1128
    set pkg_name=com.ss.android.ugc.aweme
) else if [%host_type%]==[toutiao] (
    set schemaName=snssdk143
    set pkg_name=com.ss.android.article.news
) else (
    set schemaName=sslocal
)

:: check adb connection
adb get-state

echo delete local data ...
@REM set rm_path=/sdcard/Android/data/%pkg_name%/files/_ucfiles/local
@REM adb shell "if [ -e %rm_path% ]; then rm -rR %rm_path%; fi"
adb shell rm -rf /sdcard/Android/data/%pkg_name%/files/_ucfiles/local
adb shell rm -rf /sdcard/Android/data/%pkg_name%/files/_ucfiles/Unity/local

:: execute
if [%launch_from_local%]==[1] (
    set APP_SCHEMA="%schemaName%://microgame?version=v2'&'app_id=%app_id%'&'scene=0'&'version_type=%version_type%'&'inspect=%%7B%%7D'&'tech_type=7'&'bdp_log=%%7B%%22launch_from%%22%%3A%%22stark_sdk_tools%%22%%7D"
) else (
    set APP_SCHEMA="%schemaName%://microgame?version=v2'&'app_id=%app_id%'&'scene=0'&'version_type=%version_type%'&'inspect=%%7B%%7D'&'tech_type=7"
)

echo call adb shell "am start -a android.intent.action.VIEW -d %APP_SCHEMA:"=%"
adb shell "am start -a android.intent.action.VIEW -d %APP_SCHEMA:"=%"

:: sample: adb shell "am start -a android.intent.action.VIEW -d snssdk1128://microgame?version=v2'&'app_id=tt9a4aecf7057074ae'&'scene=0'&'version_type=latest'&'inspect=%7B%7D'&'tech_type=7"

REM '&'scene=021020'
REM '&'bdp_log=%7B%22launch_from%22%3A%22desktop%22%7D
:: sample: adb shell "am start -a android.intent.action.VIEW -d snssdk1128://microgame?version=v2'&'app_id=tt9a4aecf7057074ae'&'scene=021020'&'version_type=latest'&'inspect=%7B%7D'&'bdp_log=%7B%22launch_from%22%3A%22desktop%22%7D'&'tech_type=7"

REM '&'scene=011004'
REM '&'bdp_log=%7B%22launch_from%22%3A%22my_favorite%22%7D

echo Finish.
