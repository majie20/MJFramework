if [ $# -lt 1 ]; then
    echo "Usage: $0 command"
    echo "Example: $0 devices"
    echo "Example: $0 disconnect"
    exit 1
fi

command=$@


# ADB=`which adb`
ADB=adb


echo "\$1: command:" $command


if [ ! command -v $ADB &> /dev/null ]; then
    echo "Error: adb is not installed! \$ADB: $ADB" >&2
    exit 1
fi
# if ! [ -x "$(command -v $ADB)" ]; then
#     echo 'Error: adb is not installed.' >&2
#     exit 1
# fi
# if [ ! -f $ADB ]; then
#     echo "adb not found! \$ADB: $ADB"
#     exit 1
# fi


# check adb connection
if ! adb get-state; then
    echo No connected devices!>&2
    exit 1
fi

# execute
echo "call: adb $command ..."
adb $command


echo "Finish."
