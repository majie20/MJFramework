if [ $# -lt 1 ]; then
    echo "Usage: $0 host_type app_id [version_type]"
    echo "@Param \$1 host_type options: [ douyin, toutiao ]"
    echo "@Param \$2 app_id"
    echo "@Param \$3 version_type options: [ \"\" as preview, preview, latest, current, local ]"
    echo "Example: $0 douyin tt9a4aecf7057074ae"
    echo "Example: $0 douyin tt9a4aecf7057074ae preview"
    echo "Example: $0 douyin tt9a4aecf7057074ae latest"
    echo "Example: $0 douyin tt9a4aecf7057074ae current"
    echo "Example: $0 douyin tt9a4aecf7057074ae local"
    echo "Example: $0 toutiao tt9a4aecf7057074ae"
    exit 1
fi

host_type=$1
app_id=$2
if [ $# -gt 1 ] && [ ! -z $2 ]; then
    version_type=$3
else
    version_type="preview"
fi
launch_from_local="0"
if [ $version_type = "local" ]; then
    version_type=latest
    launch_from_local="1"
fi

# ADB=`which adb`
ADB=adb


echo "\$1: host_type:" $host_type
echo "\$2: app_id:" $app_id
echo "\$3: version_type:" $version_type


if [ -z "$host_type" ]; then
    echo "host_type not valid! \$host_type: $host_type"
    exit 1
fi

if [ -z "$app_id" ]; then
    echo "app_id not valid! \$app_id: $app_id"
    exit 1
fi

if [ -z "$version_type" ]; then
    echo "version_type not valid! \$version_type: $version_type"
    exit 1
fi

if [ ! command -v $ADB &> /dev/null ]; then
    echo "Error: adb is not installed! \$ADB: $ADB" >&2
    exit 1
fi

set schemaName="sslocal"
set pkg_name="com.ss.android.ugc.aweme"
if [ $host_type = "douyin" ]; then
    schemaName="snssdk1128"
    pkg_name="com.ss.android.ugc.aweme"
elif [ $host_type = "toutiao" ]; then
    schemaName="snssdk143"
    pkg_name="com.ss.android.article.news"
else
    schemaName="sslocal"
fi

# check adb connection
adb get-state

echo delete local data ...
# set rm_path=/sdcard/Android/data/${pkg_name}/files/_ucfiles/local
# adb shell "if [ -e ${rm_path} ]; then rm -rR ${rm_path}; fi"
adb shell rm -rf /sdcard/Android/data/${pkg_name}/files/_ucfiles/local
adb shell rm -rf /sdcard/Android/data/${pkg_name}/files/_ucfiles/Unity/local

# execute
if [ $launch_from_local = "1" ]; then
    echo adb shell \""am start -a android.intent.action.VIEW -d ${schemaName}://microgame?app_id=${app_id}'&'version=v2'&'scene=0'&'version_type=${version_type}'&'inspect=%7B%7D'&'tech_type=7'&'bdp_log=%7B%22launch_from%22%3A%22stark_sdk_tools%22%7D"\"
    adb shell "am start -a android.intent.action.VIEW -d ${schemaName}://microgame?app_id=${app_id}'&'version=v2'&'scene=0'&'version_type=${version_type}'&'inspect=%7B%7D'&'tech_type=7'&'bdp_log=%7B%22launch_from%22%3A%22stark_sdk_tools%22%7D"
else
    echo adb shell \""am start -a android.intent.action.VIEW -d ${schemaName}://microgame?app_id=${app_id}'&'version=v2'&'scene=0'&'version_type=${version_type}'&'inspect=%7B%7D'&'tech_type=7"\"
    adb shell "am start -a android.intent.action.VIEW -d ${schemaName}://microgame?app_id=${app_id}'&'version=v2'&'scene=0'&'version_type=${version_type}'&'inspect=%7B%7D'&'tech_type=7"
fi

# sample: adb shell "am start -a android.intent.action.VIEW -d snssdk1128://microgame?app_id=tt9a4aecf7057074ae'&'version=v2'&'scene=0'&'version_type=latest'&'inspect=%7B%7D'&'tech_type=7"


# '&'scene=021020'
# 今日头条场景值
# 固定入口 011-  011004  小程序中心-最近使用
# 固定入口 011-  011020  小程序桌面快捷方式
# 固定入口 011-  011005  小程序中心-收藏
# 抖音场景值
# 固定入口 021-  021001  我的-小程序
# 固定入口 021-  021003  我的-收藏 tab 入口
# 固定入口 021-  021020  小程序桌面快捷方式
# 今日头条极速版场景值
# 固定入口 061-  061020  小程序桌面快捷方式
# '&'bdp_log=%7B%22launch_from%22%3A%22desktop%22%7D
# sample: adb shell "am start -a android.intent.action.VIEW -d snssdk1128://microgame?app_id=tt9a4aecf7057074ae'&'version=v2'&'scene=021020'&'version_type=latest'&'inspect=%7B%7D'&'bdp_log=%7B%22launch_from%22%3A%22desktop%22%7D'&'tech_type=7"

# '&'scene=011004'
# '&'bdp_log=%7B%22launch_from%22%3A%22my_favorite%22%7D

echo "Finish."
