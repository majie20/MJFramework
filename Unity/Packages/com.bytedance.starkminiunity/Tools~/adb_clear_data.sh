if [ $# -lt 1 ]; then
    echo "Usage: $0 pkg_name"
    echo "Example: $0 com.ss.android.ugc.aweme"
    echo "Example: $0 com.ss.android.article.news"
    exit 1
fi

pkg_name=$1

# ADB=`which adb`
ADB=adb


echo "\$1: pkg_name:" $pkg_name
echo "\$2: adb:" $ADB


if [ -z "$pkg_name" ]; then
    echo "pkg_name not valid! \$pkg_name: $pkg_name"
    exit 1
fi

if [ ! command -v $ADB &> /dev/null ]; then
    echo "Error: adb is not installed! \$ADB: $ADB" >&2
    exit 1
fi



# check adb connection
adb get-state

# execute
echo "call: adb shell pm clear $pkg_name ..."
adb shell pm clear $pkg_name


echo "Finish."
