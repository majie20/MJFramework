if [ $# -lt 1 ]; then
    echo "Usage: $0 apk_path push_to_path"
    echo "Example: $0 miniapp.apk /sdcard/Android/data/com.ss.android.ugc.aweme/files/.patchs/"
    echo "Example: $0 appbrandplugin.apk /sdcard/Android/data/com.ss.android.article.news/files/.patchs/"
    echo "Example: $0 ucgame.apk /sdcard/Android/data/com.ss.android.ugc.aweme/files/_ucfiles/local/main.apk"
    echo "Example: $0 ucgame.apk /sdcard/Android/data/com.ss.android.article.news/files/_ucfiles/local/main.apk"
    exit 1
fi

apk_path="$1"

push_to_path="$2"

# ADB=`which adb`
ADB=adb


echo "\$1: apk_path:" $apk_path
echo "\$2: push_to_path:" $push_to_path
echo "\$3: adb:" $ADB


if [ ! -f "$apk_path" ]; then
    echo "apk_path not exist! \$apk_path: $apk_path"
    exit 1
fi

if [ -z "$push_to_path" ]; then
    echo "push_to_path not valid! \$push_to_path: $push_to_path"
    exit 1
fi

if [ ! command -v $ADB &> /dev/null ]; then
    echo "Error: adb is not installed! \$ADB: $ADB" >&2
    exit 1
fi


# check adb connection
adb get-state

# execute
push_to_dir=$(dirname $push_to_path)
echo push_to_dir: $push_to_dir
if [[ $push_to_path = *"/main.apk" ]]; then
    echo Push file is \"/main.apk\". Clean local dir first ...
    echo adb shell \"rm -rf $push_to_dir\"
    adb shell "rm -rf $push_to_dir"
fi
# execute
echo push_to_path: $push_to_path
echo call: adb push \"$apk_path\" \"$push_to_path\" ...
adb push "$apk_path" "$push_to_path"


echo "Finish."
