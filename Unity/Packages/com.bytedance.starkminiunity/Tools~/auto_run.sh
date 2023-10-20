if [ $# -lt 1 ]; then
    echo "Usage: $0 apk_path [apk_type (mini or game, default: game)] [0 or default: delete data. -k: keep data]"
    echo "Example: $0 ucgame.apk game"
    echo "Example: $0 ucgame.apk game -k"
    exit 1
fi

apk_type=game
apk_path="$1"

if [ $# -gt 1 ] && [ ! -z $2 ]; then
    apk_type=$2
fi


keep_data=0
if [ $# -gt 2 ] && [ ! -z $3 ]; then
    keep_data=$3
fi

# must use certain app_id to run local push main.apk
app_id=tt9a4aecf7057074ae

ADB=adb

echo "apk_type:" $apk_type
echo "apk_path:" $apk_path
echo "keep_data:" $keep_data
echo "app_id:" $app_id

if [ ! command -v $ADB &> /dev/null ]; then
    echo "Error: adb is not installed! \$ADB: $ADB" >&2
    exit 1
fi

if [ ! -f "$apk_path" ]; then
    echo "apk_path not exist! \$apk_path: $apk_path"
    exit 1
fi

host_type=douyin
version_type=latest
schemaName=snssdk1128

force_restart=1
APP_PACKAGE_NAME="com.ss.android.ugc.aweme"
APP_LAUNCHER_ACTIVITY_NAME="com.ss.android.ugc.aweme.splash.SplashActivity"
# APP_SCHEMA=snssdk1128://microgame?app_id=${app_id}'&'version=v2'&'scene=011004'&'version_type=current'&'bdp_log=%7B%22launch_from%22%3A%22my_favorite%22%7D'&'tech_type=7'&'bdpsum=b27744e"
APP_SCHEMA="${schemaName}://microgame?version=v2'&'app_id=${app_id}'&'scene=0'&'version_type=${version_type}'&'inspect=%7B%7D'&'tech_type=7'&'bdp_log=%7B%22launch_from%22%3A%22stark_sdk_tools%22%7D"


# check adb connection
adb get-state

if [ $apk_type = "mini" ]; then
    $ADB shell am start -n ${APP_PACKAGE_NAME}/${APP_LAUNCHER_ACTIVITY_NAME}
    # echo adb push \"$apk_path\" /sdcard/Android/data/${APP_PACKAGE_NAME}/files/.patchs/
    $ADB push "$apk_path" /sdcard/Android/data/${APP_PACKAGE_NAME}/files/.patchs/
    echo force-stop app ...
    $ADB shell am force-stop ${APP_PACKAGE_NAME}
    echo wait restart app ...
    $ADB shell am start -n ${APP_PACKAGE_NAME}/${APP_LAUNCHER_ACTIVITY_NAME}
elif [ $apk_type = "game" ]; then
    if [ $keep_data = "-k" ]; then
        echo keep local data ...
    else
        echo delete local data ...
        $ADB shell rm -rf /sdcard/Android/data/${APP_PACKAGE_NAME}/files/_ucfiles/local
        $ADB shell rm -rf /sdcard/Android/data/${APP_PACKAGE_NAME}/files/_ucfiles/Unity/local
    fi
    echo push apk ...
    # echo adb push \"$apk_path\" /sdcard/Android/data/${APP_PACKAGE_NAME}/files/_ucfiles/local/main.apk
    $ADB push "$apk_path" /sdcard/Android/data/${APP_PACKAGE_NAME}/files/_ucfiles/local/main.apk
    if [ $force_restart = "1" ]; then
        echo force-stop app ...
        $ADB shell am force-stop $APP_PACKAGE_NAME
        echo wait restart app ...
        echo sleep 1 && sleep 1
        # $ADB shell monkey -p $APP_PACKAGE_NAME -c android.intent.category.LAUNCHER 1
        $ADB shell am start -n ${APP_PACKAGE_NAME}/${APP_LAUNCHER_ACTIVITY_NAME}
        echo sleep 3 && sleep 3
    fi
    echo run apk by schema ...
    # echo $ADB shell \"am start -a android.intent.action.VIEW -d ${APP_SCHEMA}\"
    $ADB shell "am start -a android.intent.action.VIEW -d ${APP_SCHEMA}"
else
    echo "unknown type: $apk_type"
fi

echo "finish."
