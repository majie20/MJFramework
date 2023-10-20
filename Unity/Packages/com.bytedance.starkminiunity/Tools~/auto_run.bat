@echo off
echo params: "%1%2%3"
if "%~1%~2%~3"=="" (
    echo Usage: %~nx0 APK_PATH [APK_TYPE: mini or game, default: game] [0 or default: delete data. -k: keep data]
    echo @Param %%1 APK_PATH
    echo @Param %%2 Optional, APK_TYPE options: game: push and run game apk. mini: push miniapp plugin for patch.
    echo @Param %%3 Optional, keep data options: 0 or default: delete data. -k: keep data.
    exit /b 1
)

REM chcp
CHCP 65001

set APK_PATH=%~1
set APK_TYPE=%2
set keep_data=%3

if "%APK_PATH%"=="" (
    echo APK_PATH error!
    exit /b 1
)

if "%APK_TYPE%"=="" (
    set APK_TYPE=game
)

if "%keep_data%"=="" (
    set keep_data=0
)

:: must use certain app_id to run local push main.apk
set app_id=tt9a4aecf7057074ae

set ADB=adb


echo APK_PATH: "%APK_PATH%"
echo APK_TYPE: %APK_TYPE%
echo app_id: %app_id%

if not exist "%APK_PATH%" (
    echo apk_path not exist! %%APK_PATH%%: "%APK_PATH%"
    exit /b 1
)

set host_type=douyin
set version_type=latest
set schemaName=sslocal
if [%host_type%]==[douyin] (
    set schemaName=snssdk1128
) else if [%host_type%]==[toutiao] (
    set schemaName=snssdk143
) else (
    set schemaName=sslocal
)

set force_restart=1
set APP_PACKAGE_NAME=com.ss.android.ugc.aweme
set APP_LAUNCHER_ACTIVITY_NAME=com.ss.android.ugc.aweme.splash.SplashActivity
REM note: in batch, use %% for the `%` char in schema string
@REM set APP_SCHEMA="snssdk1128://microgame?version=v2'&'app_id=%app_id%'&'scene=011004'&'version_type=current'&'bdp_log=%%7B%%22launch_from%%22%%3A%%22my_favorite%%22%%7D'&'tech_type=7'&'bdpsum=b27744e"
set APP_SCHEMA="%schemaName%://microgame?version=v2'&'app_id=%app_id%'&'scene=0'&'version_type=%version_type%'&'inspect=%%7B%%7D'&'tech_type=7'&'bdp_log=%%7B%%22launch_from%%22%%3A%%22stark_sdk_tools%%22%%7D"


:: check adb connection
adb get-state

if "%APK_TYPE%" == "mini" (
    %ADB% shell am start -n %APP_PACKAGE_NAME%/%APP_LAUNCHER_ACTIVITY_NAME%
    %ADB% push "%APK_PATH%" /sdcard/Android/data/%APP_PACKAGE_NAME%/files/.patchs/
    echo force-stop app ...
    %ADB% shell am force-stop %APP_PACKAGE_NAME%
    echo wait restart app ...
    @REM echo call %ADB% shell am start -n %APP_PACKAGE_NAME%/%APP_LAUNCHER_ACTIVITY_NAME%
    %ADB% shell am start -n %APP_PACKAGE_NAME%/%APP_LAUNCHER_ACTIVITY_NAME%
) else if "%APK_TYPE%" == "game" (
    if "%keep_data%"=="-k" (
        echo keep local data ...
    ) else (
        echo delete local data ...
        %ADB% shell rm -rf /sdcard/Android/data/%APP_PACKAGE_NAME%/files/_ucfiles/local
        %ADB% shell rm -rf /sdcard/Android/data/%APP_PACKAGE_NAME%/files/_ucfiles/Unity/local
    )
    echo push apk ...
    %ADB% push "%APK_PATH%" /sdcard/Android/data/%APP_PACKAGE_NAME%/files/_ucfiles/local/main.apk
    if "%force_restart%"=="1" (
        echo force-stop app ...
        %ADB% shell am force-stop %APP_PACKAGE_NAME%
        echo wait restart app ...
        timeout /t 1 /nobreak > NUL
        @REM adb shell monkey -p %APP_PACKAGE_NAME% -c android.intent.category.LAUNCHER 1
        %ADB% shell am start -n %APP_PACKAGE_NAME%/%APP_LAUNCHER_ACTIVITY_NAME%
        timeout /t 3 /nobreak > NUL
    )
    echo run apk by schema ...
    @REM echo call %ADB% shell "am start -a android.intent.action.VIEW -d %APP_SCHEMA:"=%"
    %ADB% shell "am start -a android.intent.action.VIEW -d %APP_SCHEMA:"=%"
) else (
    echo Unknown type: %APK_TYPE%
    exit /b 1
)

echo finish.
