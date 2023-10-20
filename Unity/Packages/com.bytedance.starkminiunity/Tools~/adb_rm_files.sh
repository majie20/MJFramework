if [ $# -lt 1 ]; then
    echo "Usage: $0 filepath"
    echo "Example: $0 /sdcard/Android/data/com.ss.android.ugc.aweme/files/_ucfiles"
    exit 1
fi

filepath="$1"
filepath2="$2"
filepath3="$3"

ADB=adb

LEN_THRESH=10
echo "\$1: filepath:"  $filepath
if [ -z "$filepath" ]; then
    echo "filepath not valid! \$filepath: $filepath"
    exit 1
fi
if [ ${#filepath} -lt $LEN_THRESH ]; then
    echo "filepath length not valid! too short: ${#filepath} < $LEN_THRESH! \$filepath: $filepath"
    exit 1
fi

if [ $# -gt 1 ]; then
    echo "\$2 filepath2:" $2
    if [ ${#filepath2} -lt $LEN_THRESH ]; then
        echo "filepath2 length not valid! too short: ${#filepath2} < $LEN_THRESH! \$filepath2: $filepath2"
        exit 1
    fi
fi
if [ $# -gt 2 ]; then
    echo "\$3 filepath3:" $3
    if [ ${#filepath3} -lt $LEN_THRESH ]; then
        echo "filepath3 length not valid! too short: ${#filepath3} < $LEN_THRESH! \$filepath3: $filepath3"
        exit 1
    fi
fi


if [ ! command -v $ADB &> /dev/null ]; then
    echo "Error: adb is not installed! \$ADB: $ADB" >&2
    exit 1
fi


# check adb connection
adb get-state

# execute
echo "call: adb shell rm -rf \"$filepath\" ..."
adb shell rm -rf "$filepath"
if [ $# -gt 1 ]; then
    echo "call: adb shell rm -rf \"$filepath2\" ..."
    adb shell rm -rf "$filepath2"
fi
if [ $# -gt 2 ]; then
    echo "call: adb shell rm -rf \"$filepath3\" ..."
    adb shell rm -rf "$filepath3"
fi


echo "Finish."
