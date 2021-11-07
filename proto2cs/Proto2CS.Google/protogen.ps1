# 跳转到当前powershell脚本文件目录
# $scriptpath = $PSScriptRoot
# cd $scriptpath

# 获取用户目录
# $userpath=$env:USERPROFILE
# 执行protoc生成类
.\protoc.exe --proto_path=../../Proto --csharp_out=CSFile test.proto