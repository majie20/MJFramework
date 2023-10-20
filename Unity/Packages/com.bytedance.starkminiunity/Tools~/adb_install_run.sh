if [ $# -lt 1 ]; then
    echo "Usage: $0 apk_path [pkg_name]"
    echo "Example: $0 douyin.apk com.ss.android.ugc.aweme"
    echo "Example: $0 toutiao.apk com.ss.android.article.news"
    exit 1
fi

apk_path="$1"

pkg_name=$2

# ADB=`which adb`
ADB=adb


echo "\$1: apk_path:" $apk_path
echo "\$2: pkg_name:" $pkg_name
echo "\$3: adb:" $ADB


if [ ! -f "$apk_path" ]; then
    echo "apk_path not exist! \$apk_path: $apk_path"
    exit 1
fi

if [ ! command -v $ADB &> /dev/null ]; then
    echo "Error: adb is not installed! \$ADB: $ADB" >&2
    exit 1
fi


# check adb connection
adb get-state

# install
echo "call: adb install -r \"$apk_path\" ..."
adb install -r "$apk_path"


# run app
if [ ! -z "$pkg_name" ]; then
    echo "call: adb shell monkey -p $pkg_name ..."
    adb shell monkey -p $pkg_name -c android.intent.category.LAUNCHER 1
fi


echo "Finish."
