@echo off

SET _PROJECT_PATH=..\..\
SET _PROTO_PATH=..\..\Proto
SET _UNITY_CSPD_PATH=%_PROJECT_PATH%\Unity\Assets\Scripts\Model\Protobuf\

del .\*.cs
del .\*.proto

for /f "delims=" %%i in ( 'type ProtoFilters.txt' ) do (
    echo %%i
    copy %_PROTO_PATH%\%%i
    protogen -i:%%i -o:%%~ni.cs
)

del %_UNITY_CSPD_PATH%\*.cs

copy /y .\*.cs %_UNITY_CSPD_PATH%

del .\*.cs
del .\*.proto

pause