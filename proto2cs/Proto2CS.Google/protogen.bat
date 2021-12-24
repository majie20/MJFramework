@echo off

SET _PROJECT_PATH=..\..\
SET _PROTO_PATH=..\..\Proto
SET _PROTO2CS_PATH=.\CSFile
SET _UNITY_CSPD_PATH=%_PROJECT_PATH%\Unity\Assets\Scripts\Hotfix\Protobuf\
SET _SERVER_CSPD_PATH=%_PROJECT_PATH%\Server\HttpServerTest\Protobuf\

del %_PROTO2CS_PATH%\*.cs

for /f "delims=" %%i in ( 'type ProtoFilters.txt' ) do (
    echo %%i
    protoc.exe --proto_path=%_PROTO_PATH% --csharp_out=%_PROTO2CS_PATH% %%i
)

del %_UNITY_CSPD_PATH%\*.cs
del %_SERVER_CSPD_PATH%\*.cs

copy /y %_PROTO2CS_PATH%\*.cs %_UNITY_CSPD_PATH%
copy /y %_PROTO2CS_PATH%\*.cs %_SERVER_CSPD_PATH%

del %_PROTO2CS_PATH%\*.cs

pause